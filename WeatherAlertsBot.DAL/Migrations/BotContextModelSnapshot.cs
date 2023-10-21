﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WeatherAlertsBot.DAL.Contexts;

#nullable disable

namespace WeatherAlertsBot.DAL.Migrations
{
    [DbContext(typeof(BotContext))]
    partial class BotContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SubscriberSubscriberCommand", b =>
                {
                    b.Property<int>("CommandsId")
                        .HasColumnType("int");

                    b.Property<long>("SubsrcibersChatId")
                        .HasColumnType("bigint");

                    b.HasKey("CommandsId", "SubsrcibersChatId");

                    b.HasIndex("SubsrcibersChatId");

                    b.ToTable("SubscriberSubscriberCommand");
                });

            modelBuilder.Entity("WeatherAlertsBot.DAL.Entities.Subscriber", b =>
                {
                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.HasKey("ChatId");

                    b.ToTable("Subscribers");
                });

            modelBuilder.Entity("WeatherAlertsBot.DAL.Entities.SubscriberCommand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("CommandName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("SubscriberCommands");
                });

            modelBuilder.Entity("SubscriberSubscriberCommand", b =>
                {
                    b.HasOne("WeatherAlertsBot.DAL.Entities.SubscriberCommand", null)
                        .WithMany()
                        .HasForeignKey("CommandsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WeatherAlertsBot.DAL.Entities.Subscriber", null)
                        .WithMany()
                        .HasForeignKey("SubsrcibersChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
