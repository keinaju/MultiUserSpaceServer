// using System.ComponentModel.DataAnnotations;
// using Microsoft.EntityFrameworkCore.Infrastructure;

// namespace MUS.Game.Data.Models;

// public class RoomInPool
// {
//     [Key]
//     public int PrimaryKey { get; set; }

//     public int RoomPrimaryKey { get; set; }
//     public required Room Room
//     {
//         get => _lazyLoader.Load(this, ref _room);
//         set => _room = value;
//     }

//     private readonly ILazyLoader _lazyLoader;
//     private Room _room;

//     private RoomInPool(ILazyLoader lazyLoader)
//     {
//         _lazyLoader = lazyLoader;
//     }
// }
