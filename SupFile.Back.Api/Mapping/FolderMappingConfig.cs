namespace SupFile.Back.Api.Mapping;

public class FolderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Folder, FolderModel>()
            .Map(dest => dest.SharedUsers, src => src.Shares.Select(s => s.ShareUser).Adapt<List<ApplicationUserModel>>());
        
    }
}
