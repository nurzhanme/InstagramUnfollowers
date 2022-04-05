using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;

namespace InstagramUnfollowers
{
    public class Client
    {
        private IInstaApi _instaApi;

        public async Task<string> Login(string username, string password)
        {

            _instaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(UserSessionData.ForUsername(username).WithPassword(password))
                .SetRequestDelay(RequestDelay.FromSeconds(2, 2))
                .Build();

            await _instaApi.SendRequestsBeforeLoginAsync();
            var result = await _instaApi.LoginAsync();

            if (!result.Succeeded || !_instaApi.IsUserAuthenticated)
            {
                throw new InstaException(result.Info.Message);
            }

            await _instaApi.SendRequestsAfterLoginAsync();

            var stateData = await _instaApi.GetStateDataAsStringAsync();
            return stateData;
        }

        public async Task Login(string sessionData)
        {

            _instaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(UserSessionData.Empty)
                .SetRequestDelay(RequestDelay.FromSeconds(2, 2))
                .Build();

            await _instaApi.LoadStateDataFromStringAsync(sessionData);

            if (!_instaApi.IsUserAuthenticated) throw new InstaException("Not Authorized");
        }

        public async Task<List<string>> GetNotFollowingBack(string username)
        {
            var followingResult = await _instaApi.UserProcessor.GetUserFollowingAsync(username, PaginationParameters.MaxPagesToLoad(int.MaxValue));

            if (!followingResult.Succeeded)
            {
                throw new Exception(followingResult.Info.Message);
            }

            var followingSet = followingResult.Value.Select(x => x.UserName).ToHashSet();

            var followersResult = await _instaApi.UserProcessor.GetUserFollowersAsync(username, PaginationParameters.MaxPagesToLoad(int.MaxValue));

            if (!followersResult.Succeeded)
            {
                throw new Exception(followersResult.Info.Message);
            }

            foreach (var item in followersResult.Value.Select(x => x.UserName))
            {
                followingSet.Remove(item);
            }

            return followingSet.ToList();
        }
    }
}