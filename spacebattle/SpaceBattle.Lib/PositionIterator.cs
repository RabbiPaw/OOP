using System.Windows.Input;
using Hwdtech;

namespace SpaceBattle.Lib;

public class PositionIterator : IEnumerator<object>
{
    private readonly IList<Vector> _positions;
    private int _positionIndex = 0;

    public PositionIterator()
    {
        _positions = IoC.Resolve<IList<Vector>>("Game.StartPosition");
    }

    public object Current => _positions[_positionIndex];

    public bool MoveNext()
    {
        _positionIndex = _positionIndex + 1;
        return _positionIndex < _positions.Count();
    }

    public void  Reset(){_positionIndex = 0;}

    public void Dispose(){ throw new NotImplementedException(); }


    
}