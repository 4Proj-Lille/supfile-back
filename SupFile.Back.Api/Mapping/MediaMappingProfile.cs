namespace SupFile.Back.Api.Mapping;

public class MediaMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Media, MediaModel>()
            .Map(dest => dest.Type, src => MediaTypeHelper.GetMediaTypeByExtension(src.Extension));
    }
}
