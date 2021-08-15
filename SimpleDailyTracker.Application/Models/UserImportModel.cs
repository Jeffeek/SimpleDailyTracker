using System;
using System.Text.Json.Serialization;
using SimpleDailyTracker.Application.Enums;

namespace SimpleDailyTracker.Application.Models
{
    public class UserImportModel : IComparable<UserImportModel>, IEquatable<UserImportModel>
    {
        [JsonPropertyName("User")]
        public string User { get; init; } = default!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("Status")]
        public DailyStatus Status { get; init; }

        [JsonPropertyName("Rank")]
        public int Rank { get; init; }

        [JsonPropertyName("Steps")]
        public int Steps { get; init; }

        #region Relational members

        /// <inheritdoc />
        public int CompareTo(UserImportModel other)
        {
            if (ReferenceEquals(this, other))
                return 0;

            if (ReferenceEquals(null, other))
                return 1;

            var userNameComparison = String.Compare(User, other.User, StringComparison.Ordinal);

            if (userNameComparison != 0)
                return userNameComparison;

            var statusComparison = Status.CompareTo(other.Status);

            if (statusComparison != 0)
                return statusComparison;

            var rankComparison = Rank.CompareTo(other.Rank);

            return rankComparison != 0 ? rankComparison : Steps.CompareTo(other.Steps);
        }

        #endregion

        #region Equality members

        /// <inheritdoc />
        public bool Equals(UserImportModel other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return User == other.User
                   && Status == other.Status
                   && Rank == other.Rank
                   && Steps == other.Steps;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType() && Equals((UserImportModel)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(User, (int)Status, Rank, Steps);

        #endregion
    }
}
