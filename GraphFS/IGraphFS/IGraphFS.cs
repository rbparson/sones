/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;
using sones.Library.VersionedPluginManager;
using sones.Library.Commons.VertexStore;
using sones.Library.Commons.Security;

namespace sones.GraphFS
{
    #region IGraphFSVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IGraphFS plugin versions. 
    /// Defines the min and max version for all IGraphFS implementations which will be activated used this IGraphFS.
    /// </summary>
    public static class IGraphFSVersionCompatibility
    {
        public static Version MinVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
        public static Version MaxVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
    }

    #endregion

    /// <summary>
    /// The interface for all kinds of GraphFS
    /// </summary>
    public interface IGraphFS : IVertexStore, IPluginable
    {
        #region Information Methods

        #region IsTransactionsal

        /// <summary>
        /// Determines whether this fs supports handling of transactions (The Fs has to implement ITransactionManager). 
        /// if this isn't the case, the transactionmanager of the IGraphDB is used.
        /// </summary>
        Boolean IsTransactional { get; }

        #endregion

        #region IsPersistent

        Boolean IsPersistent { get; }

        #endregion

        #region HasRevisions

        /// <summary>
        /// Determines whether this filesystem uses revisions
        /// </summary>
        Boolean HasRevisions { get; }

        #endregion

        #region HasEditions

        /// <summary>
        /// Determines whether this filesystem uses editions
        /// </summary>
        Boolean HasEditions { get; }

        #endregion

        #region GetFileSystemDescription(...)

        /// <summary>
        /// Returns the name or a description of this file system.
        /// </summary>
        /// <returns>The name or a description of this file system</returns>
        String GetFileSystemDescription();

        #endregion

        #region GetNumberOfBytes(...)

        /// <summary>
        /// Returns the size (number of bytes) of this file system
        /// </summary>
        /// <returns>The size (number of bytes) of this file system</returns>
        UInt64 GetNumberOfBytes();

        #endregion

        #region GetNumberOfFreeBytes(...)

        /// <summary>
        /// Returns the number of free bytes of this file system
        /// </summary>
        /// <returns>The number of free bytes of this file system</returns>
        UInt64 GetNumberOfFreeBytes();

        #endregion

        #endregion

        #region Grow-/Shrink-/Replicate-/WipeFileSystem

        /// <summary>
        /// This enlarges the size of a GraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToAdd">the number of bytes to add to the size of the current file system</param>
        /// <returns>New total number of bytes</returns>
        UInt64 GrowFileSystem(UInt64 myNumberOfBytesToAdd);

        /// <summary>
        /// This reduces the size of a GraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToRemove">the number of bytes to remove from the size of the current file system</param>
        /// <returns>New total number of bytes</returns>
        UInt64 ShrinkFileSystem(UInt64 myNumberOfBytesToRemove);

        /// <summary>
        /// Wipe the file system
        /// </summary>
        void WipeFileSystem();

        /// <summary>
        /// Clones the IGraphFS instance into a stream
        /// </summary>
        /// <param name="myTimeStamp">the starting timestamp of the clone. every vertex that has been created after this timestamp has to be returned</param>
        /// <returns>An enumerable of to be cloned vertices</returns>
        IEnumerable<IVertex> CloneFileSystem(DateTime myTimeStamp);

        /// <summary>
        /// Initializes a GraphFS using the replicated vertices
        /// </summary>
        /// <param name="myReplicationStream">An enumerable of vertices</param>
        /// <param name="myAppend">False: create a new FS, True: append to the current one</param>
        void ReplicateFileSystem(IEnumerable<IVertex> myReplicationStream, Boolean myAppend = false);

        #endregion

        #region misc

        /// <summary>
        /// Shutdown the graphFS
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        void Shutdown(SecurityToken mySecurityToken);

        #endregion
    }
}
