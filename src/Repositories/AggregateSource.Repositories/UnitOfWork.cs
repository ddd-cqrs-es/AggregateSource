﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AggregateSource.Repositories.Properties;

namespace AggregateSource.Repositories
{
    /// <summary>
    /// Tracks changes of attached aggregates.
    /// </summary>
    public class UnitOfWork
    {
        readonly Dictionary<string, Aggregate> _aggregates;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        public UnitOfWork()
        {
            _aggregates = new Dictionary<string, Aggregate>();
        }

        /// <summary>
        /// Attaches the specified aggregate.
        /// </summary>
        /// <param name="aggregate">The aggregate.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="aggregate"/> is null.</exception>
        public void Attach(Aggregate aggregate)
        {
            if (aggregate == null)
                throw new ArgumentNullException("aggregate");
            if (_aggregates.ContainsKey(aggregate.Identifier))
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture,
                        Resources.UnitOfWork_AttachAlreadyAdded,
                        aggregate.Root.GetType().Name, aggregate.Identifier));
            _aggregates.Add(aggregate.Identifier, aggregate);
        }

        /// <summary>
        /// Attempts to get the <see cref="Aggregate"/> using the specified aggregate identifier.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <returns>The found <see cref="Aggregate"/>, or empty if not found.</returns>
        public Optional<Aggregate> GetOptional(string identifier)
        {
            Aggregate aggregate;
            return !_aggregates.TryGetValue(identifier, out aggregate) ? 
                Optional<Aggregate>.Empty : 
                new Optional<Aggregate>(aggregate);
        }

        /// <summary>
        /// Determines whether this instance has aggregates with state changes.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has aggregates with state changes; otherwise, <c>false</c>.
        /// </returns>
        public bool HasChanges()
        {
            return _aggregates.Values.Any(aggregate => aggregate.Root.HasChanges());
        }

        /// <summary>
        /// Gets the aggregates with state changes.
        /// </summary>
        /// <returns>An enumeration of <see cref="Aggregate"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public IEnumerable<Aggregate> GetChanges()
        {
            return _aggregates.Values.Where(aggregate => aggregate.Root.HasChanges());
        }
    }
}