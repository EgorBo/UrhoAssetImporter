using System.Threading.Tasks;

namespace UrhoSharp.Viewer.Core
{
	public interface IRootAssetsDialog
	{
		Task<string> AskForRootAssetsDialog(string file);
	}
}
