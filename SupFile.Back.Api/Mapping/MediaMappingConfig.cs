namespace SupFile.Back.Api.Mapping;

public class MediaMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Media, MediaModel>()
            .Map(dest => dest.Type, src => MediaTypeHelper.GetMediaTypeByExtension(src.Extension))
            .Map(dest => dest.SharedUsers, src => src.Shares.Select(s => s.ShareUser).Adapt<List<ApplicationUserModel>>());
        
    }
}
