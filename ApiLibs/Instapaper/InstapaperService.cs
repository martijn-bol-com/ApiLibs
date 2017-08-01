﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ApiLibs;
using ApiLibs.General;
using ApiLibs.Instapaper;
using AsyncOAuth;

namespace ApiLibs.Instapaper
{
    /// <summary>
    /// Documentation partly gathered from the instapaper api website
    /// </summary>
    public class InstapaperService : Service
    {
        private string AcessToken { get; set; }

        /// <summary>
        /// Constructor to be called when you don't have a secret
        /// It is expected that <see cref="Connect"/> is called after
        /// </summary>
        public InstapaperService() : base("https://www.instapaper.com/api/1/") { }

        public InstapaperService(string clientId, string clientSecret, string token, string tokenSecret) :base("https://www.instapaper.com/api/1.1/")
        {
            OAuthUtility.ComputeHash = (key, buffer) => { using (var hmac = new HMACSHA1(key)) { return hmac.ComputeHash(buffer); } };

            Client = OAuthUtility.CreateOAuthClient(clientId, clientSecret, new AccessToken(token, tokenSecret));
            Client.BaseAddress = new Uri("https://www.instapaper.com/api/1.1/");

            //TODO fix
            //Client.Authenticator = OAuth1Authenticator.ForAccessToken(clientId, clientSecret, token, tokenSecret);

            var headerFormat = "Basic {0}";

            var authHeader = string.Format(headerFormat,
                System.Convert.ToBase64String(Encoding.Unicode.GetBytes(Uri.EscapeDataString(clientId) + ":" +
                                                                 Uri.EscapeDataString((clientSecret)))
                ));

            //AddStandardHeader("Authorization", "OAuth oauth_token=" + token + ", oauth_token_secret=" + tokenSecret);
        }

        /// <summary>
        /// Go to https://www.instapaper.com/main/request_oauth_consumer_token to get an accesstoken
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <param name="password">password of the user</param>
        /// <param name="clientId">the id of your application</param>
        /// <param name="clientSecret">secret of your application</param>
        public async Task Connect(string username, string password, string clientId, string clientSecret)
        {

            // create authorizer
            var authorizer = new OAuthAuthorizer(clientId, clientSecret);

            // get request token
            var tokenResponse = await authorizer.GetRequestToken("https://api.twitter.com/oauth/request_token");
            var requestToken = tokenResponse.Token;

            var pinCode = password;

            // get access token
            var accessTokenResponse = await authorizer.GetAccessToken("https://api.twitter.com/oauth/access_token", requestToken, pinCode);

            // save access token.
            var accessToken = accessTokenResponse.Token;
            Console.WriteLine("Key:" + accessToken.Key);
            Console.WriteLine("Secret:" + accessToken.Secret);

            //return accessToken;
        }
        /*string res = XAuth.GetAccessToken("https://www.instapaper.com/api/1/oauth/access_token", username,
                password, clientId, clientSecret);

            SetBaseUrl("https://www.instapaper.com/");
            string s = await HandleRequest("/api/1/oauth/access_token", Call.POST, XAuth.GenerateParams(username, password, clientId).FindAll(p => p.Name.StartsWith("x_auth")), headers: new List<Param>
            {
                new Param("Authorization", res)
            });

            /*string s = "";

            var client = new RestClient("https://www.instapaper.com/api/1/")
            {
                Authenticator = OAuth1Authenticator.ForRequestToken(clientId, clientSecret)
            };
            var request = new RestRequest("/oauth/access_token", Method.POST);
            request.Parameters.Add(new Parameter { Name = "x_auth_mode", Type = ParameterType.GetOrPost, Value = "client_auth" });
            request.Parameters.Add(new Parameter { Name = "x_auth_username", Type = ParameterType.GetOrPost, Value = username });
            request.Parameters.Add(new Parameter { Name = "x_auth_password", Type = ParameterType.GetOrPost, Value = password });
            var response = client.Execute(request);
            string[] respParameters = response.Content.Split('&');
            string tokenSecret = respParameters[0].Replace("oauth_token_secret=", "");
            string token = respParameters[1].Replace("oauth_token=", "");

            client.Authenticator = OAuth1Authenticator.ForAccessToken(clientId, clientSecret, token, tokenSecret);*/
        //}

        /// <summary>
        /// Lists the user's unread bookmarks, and can also synchronize reading positions.
        /// </summary>
        /// <param name="limit">Optional. A number between 1 and 500, default 25.</param>
        /// <param name="folderId">Optional. Possible values are unread (default), starred, archive, or a folder_id value from /api/1.1/folders/lists</param>
        /// <returns></returns>
        public async Task<List<Bookmark>> GetBookmarks(int limit = 25, int folderId = -1)
        {
            List<Param> param = new List<Param>();
            if (limit != 25)
            {
                param.Add(new Param("limit", limit.ToString()));
            }
            if (folderId != -1)
            {
                param.Add(new Param("folder_id", folderId.ToString()));
            }

            var s = await MakeRequest<BookmarksObject>("bookmarks/list", parameters: param);

            List<Bookmark> bookmarks = s.bookmarks;
            return bookmarks;
        }

        public async Task<List<Bookmark>> GetBookmarks(int limit, Folder folder)
        {
            return await GetBookmarks(limit, folder.folder_id);
        }

        /// <summary>
        /// Adds a new unread bookmark to the user's account.
        /// </summary>
        /// <param name="url">Required, except when using private sources</param>
        /// <param name="title">Optional. If omitted or empty, the title will be looked up by Instapaper synchronously. This will delay the action, so please specify the title if you have it.</param>
        /// <param name="description">Optional. A brief, plaintext description or summary of the article. Twitter clients often put the source tweet's text here, and Instapaper's bookmarklet puts the selected text here if the user has selected any.</param>
        /// <param name="folderId">Optional. The integer folder ID as returned by the folders/list method described below.</param>
        /// <param name="finalUrl"> Optional, default 1. Specify 1 if the url might not be the final URL that a browser would resolve when fetching it, such as if it's a shortened URL, it's a URL from an RSS feed that might be proxied, or it's likely to go through any other redirection when viewed in a browser.</param>
        /// <returns></returns>
        public async Task<Bookmark> AddBookmark(string url, string title = "", string description = "", int folderId = -1,
            string finalUrl = "")
        {
            List<Param> param = new List<Param> { new Param("url", url)};
            if (title != "")
            {
                param.Add(new Param("title", title));
            }

            if (description != "")
            {
                param.Add(new Param("description", description));
            }

            if (folderId != -1)
            {
                param.Add(new Param("folder_id", folderId.ToString()));
            }

            if (finalUrl != "")
            {
                param.Add(new Param("final_url", finalUrl));
            }

            return (await MakeRequest<List<Bookmark>>("bookmarks/add", parameters: param))[0];
        }

        /// <summary>
        /// Permanently deletes the specified bookmark. This is NOT the same as Archive. Please be clear to users if you're going to do this.
        /// </summary>
        /// <param name="bookmarkId"></param>
        /// <returns></returns>
        public async Task DeleteBookmark(int bookmarkId)
        {
            await HandleRequest("bookmarks/delete", parameters: new List<Param> {new Param("bookmark_id", bookmarkId.ToString())});
        }

        public async Task DeleteBookmark(Bookmark bm)
        {
            await DeleteBookmark(bm.bookmark_id);
        }

        /// <summary>
        /// Stars the specified bookmark.
        /// </summary>
        /// <param name="bookmarkId"></param>
        public async void StarBookmark(int bookmarkId)
        {
            await HandleRequest("bookmarks/star", parameters: new List<Param> { new Param("bookmark_id", bookmarkId.ToString()) });
        }

        /// <summary>
        /// Un-stars the specified bookmark.
        /// </summary>
        /// <param name="bookmarkId"></param>
        public async void UnstarBookmark(int bookmarkId)
        {
            await HandleRequest("bookmarks/unstar", parameters: new List<Param> { new Param("bookmark_id", bookmarkId.ToString()) });
        }

        /// <summary>
        /// Moves the specified bookmark to the Archive.
        /// </summary>
        /// <param name="bookmarkId"></param>
        public async void ArchiveBookmark(int bookmarkId)
        {
            await HandleRequest("bookmarks/archive", parameters: new List<Param> { new Param("bookmark_id", bookmarkId.ToString()) });
        }

        /// <summary>
        /// Moves the specified bookmark to the top of the Unread folder.
        /// </summary>
        /// <param name="bookmarkId"></param>
        public async void UnarchiveBookmark(int bookmarkId)
        {
            await HandleRequest("bookmarks/unarchive", parameters: new List<Param> { new Param("bookmark_id", bookmarkId.ToString()) });
        }

        public async void MoveBookmark(int bookmarkId, int folderId)
        {
            await HandleRequest("bookmars/unarchive", parameters: new List<Param>
            {
                new Param("bookmark_id", bookmarkId.ToString()),
                new Param("folder_id", folderId.ToString())
            });
        }

        /// <summary>
        /// A list of the account's user-created folders
        /// </summary>
        /// <returns></returns>
        public async Task<List<Folder>> GetFolders()
        {
            return await MakeRequest<List<Folder>>("folders/list");
        }

        /// <summary>
        /// Creates an organizational folder.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task AddFolder(string title)
        {
            await HandleRequest("folders/add", parameters: new List<Param> {new Param("title", title)});
        }

        /// <summary>
        /// Deletes the folder and moves any articles in it to the Archive.
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public async Task DeleteFolder(int folderId)
        {
            await HandleRequest("folders/delete", parameters: new List<Param> {new Param("folder_id", folderId.ToString())});
        }

        /// <summary>
        /// Finds a folder by name
        /// Library implemented
        /// </summary>
        /// <param name="foldername"></param>
        /// <returns></returns>
        public async Task<Folder> GetFolder(string foldername)
        {
            List<Folder> folders = await GetFolders();
            foreach (var folder in folders)
            {
                if (folder.title == foldername)
                {
                    return folder;
                }
            }
            throw new KeyNotFoundException("Your folder could not be found");
        }

        public async Task<List<Highlight>> GetHighlights(Bookmark bookmark) => await MakeRequest<List<Highlight>>("bookmarks/" + bookmark.bookmark_id  + "/highlights");
    }
}
