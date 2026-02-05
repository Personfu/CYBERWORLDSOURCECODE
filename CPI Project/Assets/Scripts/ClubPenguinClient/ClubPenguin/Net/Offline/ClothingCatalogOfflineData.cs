using System;
using System.Collections.Generic;
using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Client
{
    [Serializable]
    public struct ClothingCatalogOfflineData : ClubPenguin.Net.Offline.IOfflineData
    {
        public string Token { get; set; }

        public int SchemaVersion { get; set; }

        public List<CatalogItemData> MarketplaceItems;

        public Dictionary<long, long> SubmittedItemByChallenge;

        public Dictionary<long, long> ScheduledThemeChallengeIdByCatalogItemId;

        public long NextCatalogItemId;

        public int SubmissionRewardCoins;

        public long TotalItemsSold;

        public long TotalItemsPurchased;

        public int CurrentDayStamp;

        public long CurrentScheduledThemeChallengeId;

        public void Init()
        {
            if (string.IsNullOrEmpty(Token))
            {
                Token = "clothing_catalog_offline";
            }

            if (SchemaVersion <= 0)
            {
                SchemaVersion = 2;
            }

            if (MarketplaceItems == null)
            {
                MarketplaceItems = new List<CatalogItemData>();
            }

            if (SubmittedItemByChallenge == null)
            {
                SubmittedItemByChallenge = new Dictionary<long, long>();
            }

            if (ScheduledThemeChallengeIdByCatalogItemId == null)
            {
                ScheduledThemeChallengeIdByCatalogItemId = new Dictionary<long, long>();
            }

            int todayStamp = GetDayStampUtc();

            if (CurrentDayStamp != todayStamp)
            {
                CurrentDayStamp = todayStamp;
                CurrentScheduledThemeChallengeId = 0L;
            }

            if (NextCatalogItemId <= 0)
            {
                NextCatalogItemId = 1;
            }
        }

        public string GetToken()
        {
            return Token;
        }

        public static bool EnsureSchema(ref ClothingCatalogOfflineData data)
        {
            bool changed = false;

            if (string.IsNullOrEmpty(data.Token))
            {
                data.Token = "clothing_catalog_offline";
                changed = true;
            }

            if (data.SchemaVersion < 2)
            {
                data.SchemaVersion = 2;
                changed = true;
            }

            if (data.MarketplaceItems == null)
            {
                data.MarketplaceItems = new List<CatalogItemData>();
                changed = true;
            }

            if (data.SubmittedItemByChallenge == null)
            {
                data.SubmittedItemByChallenge = new Dictionary<long, long>();
                changed = true;
            }

            if (data.ScheduledThemeChallengeIdByCatalogItemId == null)
            {
                data.ScheduledThemeChallengeIdByCatalogItemId = new Dictionary<long, long>();
                changed = true;
            }

            foreach (KeyValuePair<long, long> kvp in data.SubmittedItemByChallenge)
            {
                if (kvp.Value > 0L && kvp.Key > 0L)
                {
                    if (!data.ScheduledThemeChallengeIdByCatalogItemId.ContainsKey(kvp.Value))
                    {
                        data.ScheduledThemeChallengeIdByCatalogItemId[kvp.Value] = kvp.Key;
                        changed = true;
                    }
                }
            }

            return changed;
        }

        public static int GetDayStampUtc()
        {
            return GetDayStampUtc(DateTime.UtcNow);
        }

        public static int GetDayStampUtc(DateTime utcNow)
        {
            if (utcNow.Kind != DateTimeKind.Utc)
            {
                utcNow = utcNow.ToUniversalTime();
            }
            return (utcNow.Year * 10000) + (utcNow.Month * 100) + utcNow.Day;
        }

        public static long BuildChallengeIdFromDayStamp(int dayStampUtc)
        {
            return BuildChallengeIdFromDayStamp(dayStampUtc, 0);
        }

        public static long BuildChallengeIdFromDayStamp(DateTime utcNow)
        {
            return BuildChallengeIdFromDayStamp(GetDayStampUtc(utcNow), 0);
        }

        public static long BuildChallengeIdFromDayStamp(int dayStampUtc, int challengeIndex)
        {
            return (dayStampUtc * 100L) + challengeIndex;
        }

        public static long BuildChallengeIdFromDayStamp(DateTime utcNow, int challengeIndex)
        {
            return BuildChallengeIdFromDayStamp(GetDayStampUtc(utcNow), challengeIndex);
        }
    }
}
