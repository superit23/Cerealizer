using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace Cerealizer
{
    //http://stackoverflow.com/questions/15819720/dynamically-add-c-sharp-properties-at-runtime
    public sealed class Obj404 : DynamicObject
    {
        private Dictionary<string, object> Members;

        public Obj404(Dictionary<string, object> properties)
        {
            Members = properties;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return Members.Keys;
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (Members.ContainsKey(binder.Name))
            {
                result = Members[binder.Name];
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }


        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (Members.ContainsKey(binder.Name))
            {
                Members[binder.Name] = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (Members.ContainsKey(binder.Name)
                      && Members[binder.Name] is Delegate)
            {
                result = (Members[binder.Name] as Delegate).DynamicInvoke(args);
                return true;
            }
            else
            {
                return base.TryInvokeMember(binder, args, out result);
            }
        }

    }
}
