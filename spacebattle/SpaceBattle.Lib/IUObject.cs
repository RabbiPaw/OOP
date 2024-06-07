namespace SpaceBattle.Lib;
public interface IUObject
{
    void SetProperty(string key, object value);
    object GetProperty(string key);
}

public class UObject : IUObject{
    private Dictionary<string, object> _properties;

    public UObject(){
        _properties = new Dictionary<string,object>();
    }

    public void SetProperty(string name, object value){
        _properties[name] = value;
    }
    public object GetProperty(string name){
        return _properties[name];
    }
}
