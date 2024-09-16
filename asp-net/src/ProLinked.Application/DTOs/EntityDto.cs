﻿namespace ProLinked.Application.DTOs
{
    [Serializable]
    public abstract class EntityDto
    {
        public override string ToString()
        {
            return $"[DTO: {GetType().Name}]";
        }
    }

    [Serializable]
    public abstract class EntityDto<TKey> : EntityDto
    {
        /// <summary>
        /// Id of the entity.
        /// </summary>
        public TKey Id { get; set; } = default!;

        public override string ToString()
        {
            return $"[DTO: {GetType().Name}] Id = {Id}";
        }
    }
}