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
            foldersToAdd.Add(_folder);
            MapFolder(directoryInfo, _folder, foldersToAdd, filesToAdd);
            FileSyncDal.AddFolders(foldersToAdd);
            FileSyncDal.AddFiles(filesToAdd);
        }

        private void MapFolder(DirectoryInfo directoryInfo, Folder folder, List<Folder> foldersToAdd, List<FileSyncFile> filesToAdd)
        {
            foreach (var file in directoryInfo.GetFiles())
            {
                var fileModel = new FileSyncFile()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = file.Name,
                    ParentFolderId = folder.Id,
                    Path = folder.Path + "/" + file.Name,
                    Extension = file.Extension
                };
                filesToAdd.Add(fileModel);
            }
            foreach (var directory in directoryInfo.GetDirectories())
            {
                var folderModel = new Folder()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = directory.Name,
                    ParentFolderId = folder.Id,
                    Path = folder.Path + "/" + directory.Name
                };
                foldersToAdd.Add(folderModel);
                MapFolder(directory, folderModel, foldersToAdd, filesToAdd);
            }

        }
    }
}