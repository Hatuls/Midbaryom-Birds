using System.Collections.Generic;
using UnityEngine;

namespace Midbaryom.Core
{
    [CreateAssetMenu(menuName ="ScriptableObjects/Tags/New Tag")]
    public class TagSO : ScriptableObject { }
    public interface ITaggable
    {
        IEnumerable<TagSO> Tags { get; }
    }


    public static class TagHelper
    {

        public static bool ContainTag(this ITaggable taggable, TagSO tagSO)
        {
            bool isContain = false;
            IEnumerable<TagSO> tags = taggable.Tags;
     
            foreach (var tag in tags)
            {
                isContain |= (tag == tagSO);
                if (isContain)
                    break;
            }
            return isContain;
        }

        public static bool ContainAllTags(this ITaggable taggable, TagSO[] tagsSO)
        {
            bool isContainAll = true;
            for (int i = 0; i < tagsSO.Length; i++)
            {
                isContainAll &= taggable.ContainTag(tagsSO[i]);
                if (!isContainAll)
                    break;
            }
            return isContainAll;
        }

        public static bool ContainAllTags(this ITaggable taggable, IEnumerable<TagSO> tagsSO)
        {
            bool isContainAll = true;
            foreach (var tag in tagsSO)
            {
                isContainAll &= taggable.ContainTag(tag);
                if (!isContainAll)
                    break;
            }
            return isContainAll;
        }


        public static bool ContainOneOrMoreTags(this ITaggable taggable, TagSO[] playerTagSO)
        {
            bool isContainAll = false;
            for (int i = 0; i < playerTagSO.Length; i++)
            {
                isContainAll |= taggable.ContainTag(playerTagSO[i]);
                if (isContainAll)
                    break;
            }
            return isContainAll;
        }

        public static bool ContainOneOrMoreTags(this ITaggable taggable, IEnumerable<TagSO> tags)
        {
            bool isContainAll = false;
            foreach (var tag in tags)
            {
                isContainAll |= taggable.ContainTag(tag);
                if (isContainAll)
                    break;
            }

            return isContainAll;
        }
        public static bool ContainOneOrMoreTags(this ITaggable tagable, TagSO[] tagsSO, out IReadOnlyList<TagSO> tagFound)
        {
            List<TagSO> playerTags = new List<TagSO>(tagsSO.Length);
            bool isContainAll = false;

            for (int i = 0; i < tagsSO.Length; i++)
            {
                bool containCurrent = tagable.ContainTag(tagsSO[i]);

                if (containCurrent)
                    playerTags.Add(playerTags[i]);

                isContainAll |= containCurrent;
            }

            tagFound = playerTags;
            return isContainAll;
        }

        public static bool ContainOneOrMoreTags(this ITaggable tagable, IEnumerable<TagSO> tagsSO, out IReadOnlyList<TagSO> tagFound)
        {
            List<TagSO> playerTags = new List<TagSO>();
            bool isContainAll = false;

            foreach (var tag in tagsSO)
            {
                bool containCurrent = tagable.ContainTag(tag);

                if (containCurrent)
                    playerTags.Add(tag);

                isContainAll |= containCurrent;
            }

            tagFound = playerTags;
            return isContainAll;
        }
    }
}