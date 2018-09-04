using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HighlightPlayer.Controls
{
    public class StateButtonSource : IList<ButtonState>
    {
        private List<StateButton> parents;
        private List<ButtonState> list;

        public int Count => list.Count;

        public bool IsReadOnly => false;

        public ButtonState this[int index]
        {
            get => list[index];
            set
            {
                if (value == list[index]) return;

                StateButton[] isCurrents = parents.Where(p => p.CurrentValue.Equals(list[index].Value)).ToArray();
                list[index] = value;
                foreach (StateButton parent in isCurrents) parent.CurrentValue = value.Value;
            }
        }

        public StateButtonSource()
        {
            parents = new List<StateButton>();
            list = new List<ButtonState>();
        }

        public StateButtonSource(IEnumerable<ButtonState> states)
        {
            parents = new List<StateButton>();
            list = states?.ToList() ?? new List<ButtonState>();
        }

        public void Add(ButtonState state)
        {
            list.Add(state);
        }

        public bool Remove(ButtonState state)
        {
            bool removed = list.Remove(state);

            if (removed) foreach (StateButton parent in parents) parent.CurrentValue = parent.CurrentValue;

            return removed;
        }

        public IEnumerator<ButtonState> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(ButtonState item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, ButtonState item)
        {
            list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);

            foreach (StateButton parent in parents) parent.CurrentValue = parent.CurrentValue;
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(ButtonState item)
        {
            return list.Contains(item);
        }

        public void CopyTo(ButtonState[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);

            foreach (StateButton parent in parents) parent.CurrentValue = parent.CurrentValue;
        }

        public void AddParent(StateButton parent)
        {
            parents.Add(parent);

            foreach (ButtonState state in this) state.AddParent(parent);
        }

        public void RemoveParent(StateButton parent)
        {
            parents.Remove(parent);

            foreach (ButtonState state in this) state.RemoveParent(parent);
        }
    }
}
