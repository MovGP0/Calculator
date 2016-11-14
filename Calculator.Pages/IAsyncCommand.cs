using System.Threading.Tasks;
using System.Windows.Input;

namespace Calculator.Pages
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}