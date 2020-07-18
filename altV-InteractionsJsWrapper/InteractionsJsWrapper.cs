using System;
using System.Collections.Generic;
using System.Numerics;
using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Interactions;

namespace altV_InteractionsJsWrapper
{
    public class InteractionsJsWrapper : Resource
    {
        public class Interactions : IWritable
        {
            private IInteraction[] interactions;

            public Interactions(IInteraction[] interactions) 
            {
                if (interactions == null) interactions = new IInteraction[] {};
                this.interactions = interactions;
            }

            public void OnWrite(IMValueWriter writer)
            {
                writer.BeginArray();
                
                foreach (IInteraction interaction in interactions)
                {
                    writer.BeginObject();

                    writer.Name("id");
                    writer.Value(interaction.Id);

                    writer.Name("type");
                    writer.Value(interaction.Type);

                    writer.Name("position");
                    writer.BeginObject();
                    writer.Name("x");
                    writer.Value(interaction.Position.X);
                    writer.Name("y");
                    writer.Value(interaction.Position.Y);
                    writer.Name("z");
                    writer.Value(interaction.Position.Z);
                    writer.EndObject();

                    writer.Name("dimension");
                    writer.Value(interaction.Dimension);

                    writer.Name("range");
                    writer.Value(interaction.Range);

                    writer.Name("rangeSquared");
                    writer.Value(interaction.RangeSquared);

                    writer.EndObject();
                }

                writer.EndArray();
            }
        }

        private List<Interaction> interactions = new List<Interaction>();
        private Dictionary<string, Action<IPlayer, object>> registeredEvents = new Dictionary<string, Action<IPlayer, object>>();

        public override void OnStart()
        {
            AltInteractions.Init();

            Alt.Export("registerInteractionEvent", new Action<string>(RegisterInteractionEvent));
            Alt.Export("unregisterInteractionEvent", new Action<string>(UnregisterInteractionEvent));
            Alt.Export("createInteraction", new Func<long, long, Vector3, int, int, ulong>(CreateInteraction));
            Alt.Export("setInteractionPosition", new Action<long, long, Vector3>(SetInteractionPosition));
            Alt.Export("setInteractionRange", new Action<long, long, int>(SetInteractionRange));
            Alt.Export("setInteractionDimension", new Action<long, long, int>(SetInteractionDimension));
            Alt.Export("removeInteraction", new Action<long, long>(RemoveInteraction));
        }

        private void RegisterInteractionEvent(string name)
        {
            if (registeredEvents.ContainsKey(name)) return;

            Action<IPlayer, object> action = new Action<IPlayer, object>(new Action<IPlayer, object>(async (player, value) => {
                IInteraction[] interactions = await AltInteractions.FindInteractions(player.Position, player.Dimension);
                Alt.Emit(name, player, new Interactions(interactions), value);
            }));

            Alt.OnClient(name, new Action<IPlayer, object>(action));

            registeredEvents.Add(name, action);
        }

        private void UnregisterInteractionEvent(string name)
        {
            if (!registeredEvents.ContainsKey(name)) return;

            Alt.OffClient(name, registeredEvents[name]);

            registeredEvents.Remove(name);
        }

        private Interaction GetInteraction(long type, long id)
        {
            return interactions.Find((item) => item.Type == (ulong) type && item.Id == (uint) id);
        }

        private ulong CreateInteraction(long type, long id, Vector3 position, int dimension, int range)
        {
            Interaction interaction = new Interaction((ulong) type, (ulong) id, position, dimension, (uint) range);
            AltInteractions.AddInteraction(interaction);
            interactions.Add(interaction);

            return interaction.Id;
        }

        private void SetInteractionPosition(long type, long id, Vector3 position)
        {
            Interaction interaction = GetInteraction(type, id);

            if (interaction == null) return;

            interaction.Position = position;
        }

        private void SetInteractionRange(long type, long id, int range)
        {
            Interaction interaction = GetInteraction(type, id);

            if (interaction == null) return;

            interaction.Range = (uint) range;
        }

        private void SetInteractionDimension(long type, long id, int dimension) 
        {
            Interaction interaction = GetInteraction(type, id);

            if (interaction == null) return;

            interaction.Dimension = dimension;
        }

        private void RemoveInteraction(long type, long id)
        {
            Interaction interaction = GetInteraction(type, id);

            if (interaction == null) return;

            AltInteractions.RemoveInteraction(interaction);
            interactions.Remove(interaction);
        }

        public override void OnStop()
        {
            AltInteractions.Dispose();
        }
    }
}
