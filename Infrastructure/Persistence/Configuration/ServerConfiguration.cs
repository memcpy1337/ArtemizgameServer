using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class ServerConfiguration : IEntityTypeConfiguration<Server>
{
    public void Configure(EntityTypeBuilder<Server> builder)
    {
        builder.HasIndex(x => x.Id).IsUnique();

        builder.HasIndex(x => x.ServerId).IsUnique();

        builder.HasIndex(x => x.MatchId).IsUnique(false);
    }
}