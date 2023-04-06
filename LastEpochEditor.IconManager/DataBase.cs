using Newtonsoft.Json;

namespace LastEpochEditor.IconManager;

internal class DataBase
{
	[JsonProperty(PropertyName = "itemTypes")]
	public IEnumerable<ItemType> Items { get; set; }

	[JsonProperty(PropertyName = "uniques")]
	public IEnumerable<Item> Uniques { get; set; }
}

internal class ItemType
{
	[JsonProperty(PropertyName = "BaseTypeName")]
	public string Name { get; set; }

	[JsonProperty(PropertyName = "subItems")]
	public IEnumerable<Item> SubItems { get; set; }
}

internal class Item
{
	[JsonProperty(PropertyName = "name")]
	public string Name { get; set; }

	[JsonProperty(PropertyName = "cannotDrop")]
	public bool CannotDrop { get; set; }
}