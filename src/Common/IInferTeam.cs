using System;
using System.Collections.Generic;
using Common;

namespace DataSources
{
    public interface IInferTeam<T>
    {
        IEnumerable<Participant> InferParticipants(IEnumerable<T> data);

        IEnumerable<Participant> InferParticipants(IEnumerable<T> data, Func<T, bool> predicate);
    }
}