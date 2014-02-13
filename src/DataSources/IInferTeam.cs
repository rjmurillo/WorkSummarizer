using System;
using System.Collections.Generic;
using Common;

namespace DataSources
{
    public interface IInferTeam<T>
    {
        IEnumerable<Participant> InferParticipants(IEnumerable<T> data);

        IEnumerable<Participant> InferParticipants(IEnumerable<T> workItems, Func<T, bool> predicate);
    }
}