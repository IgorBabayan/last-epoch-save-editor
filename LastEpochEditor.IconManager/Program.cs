using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using LastEpochEditor.IconManager;
using Newtonsoft.Json;
using ShellProgressBar;
using System.Runtime.InteropServices;

#pragma warning disable CS8600
#pragma warning disable CS8604

internal class Program
{
	const string DATA_URL = "https://planner-assets.com/last-epoch/build/game/data.json";
	const string URL_FORMAT = "https://planner-assets.com/last-epoch/webp/{0}/{1}.webp";
	const string DIRECTORY_NAME = "Equipment icons";

	private static readonly ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

	private static async Task Main(string[] args)
	{
		if (Directory.Exists(DIRECTORY_NAME))
			Directory.Delete(DIRECTORY_NAME, true);
		Directory.CreateDirectory(DIRECTORY_NAME);

		try
		{
			DataBase db;
			using (var clinet = new HttpClient())
			{
				using (var request = await clinet.GetAsync(DATA_URL))
				{
					var json = await request.Content.ReadAsStringAsync();
					db = JsonConvert.DeserializeObject<DataBase>(json);
				}
			}

			var options = new ProgressBarOptions
			{
				ForegroundColorDone = ConsoleColor.DarkGreen,
				BackgroundColor = ConsoleColor.DarkGray,
				BackgroundCharacter = '─',
				EnableTaskBarProgress = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
			};
			using (var mainProgress = new ProgressBar(2, "Download: images", options))
			{
				Parallel.Invoke(() => DownloadItems(db, mainProgress), () => DownloadUniques(db, mainProgress));
			}

			var files = Directory.GetFiles(DIRECTORY_NAME, "*.webp");
			foreach (var file in files)
				File.Delete(file);

			Console.WriteLine();
			Console.Write("Press any key...");
			Console.ReadKey();
		}
		catch (Exception e)
		{
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(e.Message);
			Console.WriteLine();

			var inner = e.InnerException;
			while (inner != null)
			{
				Console.WriteLine(inner.Message);
				inner = inner.InnerException;
			}
			Console.ForegroundColor = ConsoleColor.White;
		}
	}

	public static void DownloadItems(DataBase db, ProgressBar mainProgressBar)
	{
		var rootItems = db.Items.Where(x => !string.Equals(x.Name, "Affix Shard", StringComparison.InvariantCultureIgnoreCase));
		var itemsCount = rootItems.Count();
		var options = new ProgressBarOptions
		{
			DenseProgressBar = true,
			ForegroundColor = ConsoleColor.Cyan,
			ForegroundColorDone = ConsoleColor.DarkGreen,
			ProgressCharacter = '─',
			BackgroundColor = ConsoleColor.DarkGray,
			CollapseWhenFinished = true
		};
		Parallel.ForEach(rootItems, item =>
		{
			var items = item.SubItems.Where(x => !x.CannotDrop);
			var subItemsCount = items.Count();
			using (var childProgressBar = mainProgressBar.Spawn(subItemsCount, $"Download: {item.Name}", options))
			{
				Parallel.For(0, subItemsCount, index =>
				{
					var subItem = items.ElementAt(index);
					string filePath;
					using (var client = new HttpClient())
					{
						using (var request = client.GetAsync(string.Format(URL_FORMAT, item.Name.ToPlannerURL(), subItem.Name.ToPlannerURL())).Result)
						{
							var response = request.Content.ReadAsByteArrayAsync().Result;
							filePath = Path.Combine(DIRECTORY_NAME, $"{subItem.Name}.webp");

							_readWriteLock.EnterWriteLock();
							try
							{
								File.WriteAllBytes(filePath, response);
							}
							finally
							{
								_readWriteLock.ExitWriteLock();
							}
						}
					}

					var pngFileName = Path.Combine(DIRECTORY_NAME, $"{subItem.Name}.png");
					_readWriteLock.EnterWriteLock();
					try
					{
						using (var pngFileStream = new FileStream(pngFileName, FileMode.Create, FileAccess.Write))
						{
							using (var imageFactory = new ImageFactory(true))
							{
								imageFactory.Load(filePath).Format(new PngFormat()).Quality(100).Save(pngFileStream);
							}
						}
					}
					finally
					{
						_readWriteLock.ExitWriteLock();
						childProgressBar.Tick();
					}
				});
			}
		});

		mainProgressBar.Tick();
	}

	public static void DownloadUniques(DataBase db, ProgressBar mainProgressBar)
	{
		var uniquesCount = db.Uniques.Count();
		var options = new ProgressBarOptions
		{
			DenseProgressBar = true,
			ForegroundColor = ConsoleColor.Cyan,
			ForegroundColorDone = ConsoleColor.DarkGreen,
			ProgressCharacter = '─',
			BackgroundColor = ConsoleColor.DarkGray,
			CollapseWhenFinished = true
		};

		Parallel.ForEach(db.Uniques, item =>
		{
			using (var childProgressBar = mainProgressBar.Spawn(uniquesCount, $"Download: {item.Name}", options))
			{
				string filePath;
				using (var client = new HttpClient())
				{
					var url = string.Format(URL_FORMAT, "uniques", item.Name.ToPlannerURL());
					using (var request = client.GetAsync(url).Result)
					{
						var response = request.Content.ReadAsByteArrayAsync().Result;
						filePath = Path.Combine(DIRECTORY_NAME, $"{item.Name}.webp");

						_readWriteLock.EnterWriteLock();
						try
						{
							File.WriteAllBytes(filePath, response);
						}
						finally
						{
							_readWriteLock.ExitWriteLock();
						}
					}
				}

				var pngFileName = Path.Combine(DIRECTORY_NAME, $"{item.Name}.png");
				_readWriteLock.EnterWriteLock();
				try
				{
					using (var pngFileStream = new FileStream(pngFileName, FileMode.Create, FileAccess.Write))
					{
						using (var imageFactory = new ImageFactory(true))
						{
							imageFactory.Load(filePath).Format(new PngFormat()).Quality(100).Save(pngFileStream);
						}
					}
				}
				finally
				{
					_readWriteLock.ExitWriteLock();
					childProgressBar.Tick();
				}
			}
		});

		mainProgressBar.Tick();
	}
}