﻿using System.Collections.Generic;

namespace ApiLibs.Instapaper
{
    class InstapaperService : Service
    {
        private string AcessToken { get; set; }

        public InstapaperService(string username, string password)
        {
            
        }

        public InstapaperService(string accessToken)
        {
            AddStandardHeader("Authorization", accessToken);
        }

        public void Connect()
        {
            
        }

        public async void GetBookmarks(int limit = 25, int folderId = -1)
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
            await HandleRequest("/api/1/bookmarks/list", parameters: param);
        }

        public async void AddBookmark(string url, string title = "", string description = "", int folderId = -1,
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

            await HandleRequest("/bookmarks/add", parameters: param);
        }

        public async void DeleteBookmark(int bookmarkId)
        {
            await HandleRequest("/bookmarks/delete", parameters: new List<Param> {new Param("bookmark_id", bookmarkId.ToString())});
        }

        public async void StarBookmark(int bookmarkId)
        {
            await HandleRequest("/bookmarks/star", parameters: new List<Param> { new Param("bookmark_id", bookmarkId.ToString()) });
        }

        public async void UnstarBookmark(int bookmarkId)
        {
            await HandleRequest("/bookmarks/unstar", parameters: new List<Param> { new Param("bookmark_id", bookmarkId.ToString()) });
        }

        public async void ArchiveBookmark(int bookmarkId)
        {
            await HandleRequest("/bookmarks/archive", parameters: new List<Param> { new Param("bookmark_id", bookmarkId.ToString()) });
        }

        public async void UnarchiveBookmark(int bookmarkId)
        {
            await HandleRequest("/bookmarks/unarchive", parameters: new List<Param> { new Param("bookmark_id", bookmarkId.ToString()) });
        }

        public async void MoveBookmark(int bookmarkId, int folderId)
        {
            await HandleRequest("/bookmars/unarchive", parameters: new List<Param>
            {
                new Param("bookmark_id", bookmarkId.ToString()),
                new Param("folder_id", folderId.ToString())
            });
        }

        public async void GetFolders()
        {
            await HandleRequest("/folders/list");
        }

        public async void AddFolder(string title)
        {
            await HandleRequest("/folders/add", parameters: new List<Param> {new Param("title", title)});
        }

        public async void Delete(int folderId)
        {
            await HandleRequest("/folders/delete", parameters: new List<Param> {new Param("folder_id", folderId.ToString())});
        }
    }
}
