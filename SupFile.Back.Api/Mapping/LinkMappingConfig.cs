namespace SupFile.Back.Api.Mapping;

public class LinkMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Link, PendingInvitationModel>()
            .Map(dest => dest.ItemName,
                src => src.ShareLinkFile != null ? src.ShareLinkFile.Name : src.ShareLinkFolder != null ? src.ShareLinkFolder.Name : null)
            .Map(dest => dest.ItemOwnerId,
                src => src.ShareLinkFile != null ? (int?)src.ShareLinkFile.OwnerId : src.ShareLinkFolder != null ? (int?)src.ShareLinkFolder.OwnerId : null)
            .Map(dest => dest.ItemExtension,
                src => src.ShareLinkFile != null ? src.ShareLinkFile.Extension : null);
    }
}
