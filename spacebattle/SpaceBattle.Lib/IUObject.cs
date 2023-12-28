using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceBattle.Lib;
public interface IUObject
{
    void set_property (string key, object value);
    object get_property (string key);       
}
