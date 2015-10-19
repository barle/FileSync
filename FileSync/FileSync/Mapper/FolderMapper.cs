using FileSync.Models;
using FileSyncFile = FileSync.Models.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using FileSync.DAL;

namespace FileSync.Mapper
{
    public class FolderMapper
    {
        private Folder _folder;

        public FolderMapper(Folder folder)
        {
            _folder = folder;
        }

        public void Map()
        {
            var foldersToAdd = new List<Folder>();
            var filesToAdd = new List<FileSyncFile>();
            var directoryInfo = new DirectoryInfo(_folder.Path);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            _folder.Id = Guid.NewGuid().ToString();
            _folder.InsertionDate = DateTime.Now;
            foldersToAdd.Add(_folder);
            var folderSize = MapFolder(directoryInfo, _folder, foldersToAdd, filesToAdd);
            _folder.Size = folderSize;
            FileSyncDal.Instance.AddFolders(foldersToAdd);
            FileSyncDal.Instance.AddFiles(filesToAdd);
        }

        // returns the folder size and add to the foldersToAdd list all the inside folders and same for the files to the filesToAdd list
        private long MapFolder(DirectoryInfo directoryInfo, Folder folder, List<Folder> foldersToAdd, List<FileSyncFile> filesToAdd)
        {
            var size = 0L;
            foreach (var file in directoryInfo.GetFiles())
            {
                var fileModel = new FileSyncFile()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = file.Name,
                    ParentFolderId = folder.Id,
                    Path = folder.Path + "/" + file.Name,
                    Extension = file.Extension,
                    InsertionDate = DateTime.Now
                };
                filesToAdd.Add(fileModel);
                size += file.Length;
            }
            foreach (var directory in directoryInfo.GetDirectories())
            {
                var folderModel = new Folder()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = directory.Name,
                    ParentFolderId = folder.Id,
                    Path = folder.Path + "/" + directory.Name,
                    InsertionDate = DateTime.Now,
                };
                foldersToAdd.Add(folderModel);
                var folderSize = MapFolder(directory, folderModel, foldersToAdd, filesToAdd);
                folderModel.Size = folderSize;
                size += folderSize;
            }
            return size;
        }
    }
}