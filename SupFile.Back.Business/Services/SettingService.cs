using System.Transactions;
using SupFile.Back.Core.Enums;

namespace SupFile.Back.Business.Services;

public class SettingService : ISettingService
{
    private readonly AppSettings _appSettings;
    
    public SettingService( IOptions<AppSettings> appSettings)
    {
            _appSettings = appSettings.Value;
    }

    public async Task<Result<SettingDto>> GetSettingsAsync()
    {
        var result = new SettingDto
        {
            AllocatedSpace = _appSettings.AllocatedSpace
        };
        
        return Result.Ok(result);
    }
}
