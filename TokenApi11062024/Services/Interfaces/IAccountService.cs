using TokenApi11062024.Model.Binding;
using TokenApi11062024.Model.ViewModel;

namespace TokenApi11062024.Services.Interfaces
{
    public interface IAccountService
    {
        Task<TokenViewModel> GetToken(LoginBinding model);
    }
}