using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class PhotoRepository(AppDbContext context) : IPhotoRepository
{
    public async Task<PhotoDto?> AddPhotoAsync(PhotoDto photoDto)
    {
        var member = await context.Members.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == photoDto.MemberId);
        if (member == null)
        {
            return null;
        }

        var photo = new Photo
        {
            Url = photoDto.Url,
            PublicId = photoDto.PublicId,
            MemberId = photoDto.MemberId
        };

        if (member.ImageUrl == null)
        {
            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;
        }

        member.Photos.Add(photo);

        int result = await context.SaveChangesAsync();

        if (result > 0)
        {
            photoDto.Id = photo.Id;
            return photoDto;
        }
        else
        {
            return null;
        }
    }

    public async Task<int> DeletePhoto(string memberId, int photoId)
    {
        var member = await context.Members
             .Include(x => x.User)
             .SingleOrDefaultAsync(x => x.Id == memberId);

        if (member == null) return -1;

        var photo = await context.Photos.FirstOrDefaultAsync(x => x.Id == photoId);
        if (photo == null) return -1;

        member.Photos.Remove(photo);
        int result = await context.SaveChangesAsync();
        return result;

    }

    public async Task<int> SetMainPhotoAsync(string memberId, int photoId)
    {
        var member = await context.Members
             .Include(x => x.User)
             .SingleOrDefaultAsync(x => x.Id == memberId);

        if (member == null) return -1;

        var photo = await context.Photos.FirstOrDefaultAsync(x => x.Id == photoId);
        if (photo == null) return -1;

        member.ImageUrl = photo.Url;
        member.User.ImageUrl = photo.Url;
        context.Members.Update(member);
        int result = await context.SaveChangesAsync();
        return result;
    }
}
