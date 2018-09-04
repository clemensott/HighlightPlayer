using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    public class ButtonState
    {
        private const string unkownName = "Unkown";

        protected string name;
        protected List<StateButton> parents;
        private Action<StateButton, DrawingContext> noMouseOn, mouseOn, mouseClick;

        public Enum Value { get; private set; }

        public Action<StateButton, DrawingContext> MouseOut
        {
            get { return noMouseOn; }
            set
            {
                if (value == noMouseOn) return;

                noMouseOn = value;
                foreach (StateButton parent in parents) parent.InvalidateVisual();
            }
        }

        public Action<StateButton, DrawingContext> MouseOn
        {
            get { return mouseOn; }
            set
            {
                if (value == mouseOn) return;

                mouseOn = value;
                foreach (StateButton parent in parents) parent.InvalidateVisual();
            }
        }

        public Action<StateButton, DrawingContext> MouseClick
        {
            get { return mouseClick; }
            set
            {
                if (value == mouseClick) return;

                mouseClick = value;
                foreach (StateButton parent in parents) parent.InvalidateVisual();
            }
        }

        public ButtonState(Enum value) : this(value, unkownName, null, null, null)
        {
        }

        public ButtonState(Enum value, string name) : this(value, name, null, null, null)
        {
        }

        public ButtonState(Enum value, Action<StateButton, DrawingContext> noMouseOn,
            Action<StateButton, DrawingContext> mouseOn, Action<StateButton, DrawingContext> mouseClick)
            : this(value, unkownName, noMouseOn, mouseOn, mouseClick)
        {
        }

        public ButtonState(Enum value, string name, Action<StateButton, DrawingContext> noMouseOn,
            Action<StateButton, DrawingContext> mouseOn, Action<StateButton, DrawingContext> mouseClick)
        {
            parents = new List<StateButton>();
            Value = value;

            this.name = name;
            this.noMouseOn = noMouseOn;
            this.mouseOn = mouseOn;
            this.mouseClick = mouseClick;
        }

        public virtual void RenderMouseOut(StateButton sender, DrawingContext context)
        {
            noMouseOn?.Invoke(sender, context);
        }

        public virtual void RenderMouseOn(StateButton sender, DrawingContext context)
        {
            mouseOn?.Invoke(sender, context);
        }

        public virtual void RenderMouseClick(StateButton sender, DrawingContext context)
        {
            mouseClick?.Invoke(sender, context);
        }

        public void AddParent(StateButton parent)
        {
            parents.Add(parent);
        }

        public void RemoveParent(StateButton parent)
        {
            parents.Remove(parent);
        }

        public override string ToString()
        {
            return Value == null ? name : Value.GetType().Name + "." + Value;
        }
    }
}
