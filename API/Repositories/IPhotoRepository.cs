using API.DTOs;

namespace API.Repositories;

public interface IPhotoRepository
{
    Task<PhotoDto?> AddPhotoAsync(PhotoDto photo);

    Task<int> SetMainPhotoAsync(string memberId, int photoId);

    Task<int> DeletePhoto(string memberId, int photoId);
}
