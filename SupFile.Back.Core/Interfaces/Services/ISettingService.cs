using SupFile.Back.Core.Dto;
using SupFile.Back.Core.Enums;

namespace SupFile.Back.Core.Interfaces.Services;

public interface ISettingService
{
    Task<Result<SettingDto>> GetSettingsAsync();
}
