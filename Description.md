#Design Description
The solution is divided into 3 projects. The first one is for the **FileBrowser** web app. The second is for the **FileSystemModels** and the third is for unit testing.

##The FileSystemModels
The FileSystemModels are derived classes of FileSystemModelBase, an abstract class that contains the declaration of all the functions a model item would need and contain common fields and methods that are universal for all. Every item has a Parent, Children and an Address which is generated from the parent's address and the item's position inside the parent's children. This implementation serves as a kind of routing (I even used a similiar format to regular IPs) which helps reaching items faster in a complex hierarchy. Dumping everything into a collection seemed easy but not a very optimised way of implementation so I rather avoided that one. Because of the possible new item type which is neither a folder or a file, I designed the system so that files can have children as well, this way we could add any kind of new model to it. Both the FileItem and DirectoryItem need two parameters before construction, a path and a parent item. The constructor then creates an Info object as a way of composition that gets refreshed every time the getter is accessed to make sure the data is up to date. The parent parameter is used to set the item's parent and to add itself to the parent's children.

**List of Abstract Methods:**
-Name {get}: This abstract property makes it possible for further derived classes to have custom logic on returning the name. (for example the existing ones return with the Info.Name field)

-GetPath(): Does the same thing as Name but with the path of the item. I avoided using a property here because the name Path would conflict.

-GetSizeInBytes(): Gets the file's size in bytes, for directories it returns the sum of items contained in it.

-Delete(): Tries to delete the item and it's children. It is possible that some files are locked, in that case it skips them and deletes the ones that are free.

-Move(): Moves and item and the file it represents to the new parent and it's path. Currently not implemented for directories because it wasn't mentioned in the task.

-Rename()

-CreateCopy(): Creates a copy of a file or a deep copy of a directory.

-ProcessDownload() && ProcessUpload(): Currently unused but it was requested for new types.

##The FileBrowser
I don't have much experience with Asp and web development and I do not have the time to hook up the views to the controllers properly but my approach would be the following:
I would further divide the browser's page into two sections. One for the item tree and one for the contents. These PartialViews could be updated with AJAX asynchronously to avoid fully loading the page after certain events. Selecting an item in the tree would fire a onSelect event that loads in a new content view for the item based on the id. This would require some kind of switch logic if we want to show different kind of data and options to the user based on item type. The file tree view should be refreshed when a rename, move upload or any kind of modification of the structures happens. The app should also have a thread that scans the storage folder for changes and maps new items before updating the tree.

The initial mapping of the root folder is done in the FileSystemManager class. This is a workaround to compensate for the lack of a DB which could provide a "root" based on user data. This way it could be stored in a model while the manager only serves as factory. Configurations could be accessed from the ConfigurationGetter class to wrap all the config logic into one class for better maintenance. Currently it only returns hardcoded data because I did not have the capacity to fiddle with the configurations as well.

Current state: The actions for downloading and uploading files are tested and working fine. The TreeView represents the item structures correctly, but making it responsive and fully functional is too big of a task for my current situation. I try to make it work but can't promise going through with it because I'm also learning the front end things on the fly.
