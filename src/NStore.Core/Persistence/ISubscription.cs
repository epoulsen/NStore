﻿using System;
using System.Threading.Tasks;

namespace NStore.Core.Persistence
{
    public interface ISubscription
    {
        /// <summary>
        /// <para>
        /// Called when reading of <see cref="IChunk"/> starts, it can be used to initialize
        /// the component.
        /// </para>
        /// </summary>
        /// <param name="indexOrPosition"></param>
        /// <returns></returns>
        Task OnStartAsync(long indexOrPosition);

        /// <summary>
        /// <para>
        /// Handles the next <see cref="IChunk"/> in the persistence layer, and returns true
        /// if it want the caller to read again next data.
        /// </para>
        /// </summary>
        /// <param name="chunk"></param>
        /// <returns>False if this component does not want any more <see cref="IChunk"/> to be 
        /// dispatched, true if it is everything ok and the caller should continue reading next <see cref="IChunk"/></returns>
        Task<bool> OnNextAsync(IChunk chunk);

        /// <summary>
        /// <para>
        /// Called when the reading is complete.
        /// </para>
        /// </summary>
        /// <param name="indexOrPosition"></param>
        /// <returns></returns>
        Task CompletedAsync(long indexOrPosition);

        /// <summary>
        /// <para>
        /// Called when reading of chunks stops, it is usually determines by the return
        /// value of the <see cref="OnNextAsync(IChunk)"/>.
        /// </para>
        /// </summary>
        /// <param name="indexOrPosition"></param>
        /// <returns></returns>
        Task StoppedAsync(long indexOrPosition);

        /// <summary>
        /// <para>
        /// Called when there is an exception reading or dispatching the next chunk. The real
        /// concrete subscription can then take any action it determines to be done to recover
        /// from the error.
        /// </para>
        /// </summary>
        /// <param name="indexOrPosition"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        Task OnErrorAsync(long indexOrPosition, Exception ex);
    }

    public delegate Task<bool> ChunkProcessor(IChunk chunk);
}