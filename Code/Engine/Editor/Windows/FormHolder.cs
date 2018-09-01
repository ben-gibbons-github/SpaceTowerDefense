#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class FormHolder
    {
        public List<Form> FormChildren = new List<Form>();

        public virtual Form AddForm(Form NewForm)
        {
            FormChildren.Add(NewForm);
            NewForm.Create(this);
            return NewForm;
        }

        public virtual void RemoveForm(Form DeleteForm)
        {
            DeleteForm.Remove();
            FormChildren.Remove(DeleteForm);
        }
    }
}
#endif