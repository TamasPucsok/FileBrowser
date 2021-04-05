using System;
using System.Collections.Generic;
using System.Linq;

namespace FileSystemModels
{
    public abstract class FileSystemItemBase
    {
        //name, path and additional data like owner user, needed permissions etc. could come from a DB.
        protected string address;
        protected FileSystemItemBase parent;
        protected List<FileSystemItemBase> children;

        protected FileSystemItemBase(FileSystemItemBase parent)
        {
            parent?.Children.Add(this);
            this.parent = parent;

            this.children = new List<FileSystemItemBase>();
            CalculateAddresses();
        }

        public string Address { get => address; protected set => address = value; }
        public FileSystemItemBase Parent { get => parent; }
        public List<FileSystemItemBase> Children { get => children; }

        public abstract string Name { get; }

        public abstract string GetPath();

        public abstract long GetSizeInBytes();

        public abstract bool Delete();

        public abstract bool Move(FileSystemItemBase item, bool bOverWrite);

        public abstract void Rename(string newName);

        public abstract void CreateCopy();

        /// <returns>The full path to a possible new copy of the file.</returns>
        protected abstract string GetNewCopyName();

        public virtual void ProcessDownload() { }

        public virtual void ProcessUpload() { }

        /// <summary>
        /// Calculates the address of the item based on the Parent routing.
        /// </summary>
        /// <returns>The address of the item.</returns>
        public void CalculateAddresses()
        {
            if (parent == null)
            {
                Address = "0";
                return;
            }

            Address = parent.Address + "." + parent.Children.FindIndex(x => x == this);
            Children.ForEach(x => x.CalculateAddresses());
        }

        /// <summary>
        /// Searches for an item on the given address and validates it using a hash.
        /// </summary>
        /// <param name="targetAddress">The address we search for.</param>
        /// <param name="pathHash">The hash of the searched item's path, used for validation.</param>
        /// <returns>The searched item.</returns>
        public FileSystemItemBase FindSubItem(string targetAddress, ref int pathHash)
        {
            string[] addressSplit = this.address.Split(".");
            string[] targetAddressSplit = targetAddress.Split(".");

            if (addressSplit.SequenceEqual(targetAddressSplit) && this.GetPath().GetHashCode() == pathHash)
            {
                return this;
            }

            bool bTargetIsThisWay = IsAddressThisWay(targetAddress);

            if (bTargetIsThisWay && Children.Count - 1 >= int.Parse(targetAddressSplit[addressSplit.Length]))
            {
                return Children[int.Parse(targetAddressSplit[addressSplit.Length])].FindSubItem(targetAddress, ref pathHash);
            }

            return null;
        }

        /// <summary>
        /// Checks if the given address is on this object's adress.
        /// </summary>
        protected bool IsAddressThisWay(string targetAddress)
        {
            string[] addressSplit = this.address.Split(".");
            string[] targetAddressSplit = targetAddress.Split(".");

            for (int i = 0; i < addressSplit.Length; i++)
            {
                if (addressSplit[i] != targetAddressSplit[i])
                    return false;
            }

            return true;
        }

        protected void RemoveReferencesInParent()
        {
            parent?.Children.RemoveAll(x => x == this);
        }
    }
}
