<Query Kind="Program">
  <Namespace>System.ComponentModel</Namespace>
</Query>


void Main()
{
	
}

// Define other methods and classes here


class TestStruct : INotifyPropertyChanged
{
	public int Dummy { get; set; }
	
	public event PropertyChangedEventHandler PropertyChanged;

    protected void NotifyPropertyChanged(string property)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
	
	protected void ListenSubstructureChanged(INotifyPropertyChanged property, string name)
    {
        property.PropertyChanged += (object caller, PropertyChangedEventArgs args) => { NotifyPropertyChanged(name); };
    }
}

class TestObject : TestStruct
{
	private TestStruct _dummy1;
  	
	public TestStruct Dummy1
	{
		get { return this._dummy1; }
		set 
		{ 
			this._dummy1 = value;
			NotifyPropertyChanged("Dummy1");
			ListenSubstructureChanged(this._dummy1, "Dummy1");
		}
	}
}