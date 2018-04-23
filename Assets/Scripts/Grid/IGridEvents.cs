namespace Search_Shell.Grid
{
    public interface IGridEvents
    {
       void OnFinishedGravity(); 

       void OnLoadNextLevel();

       void OnLoadNextSubLevel(string name);
    }
}