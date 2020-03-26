using Microsoft.EntityFrameworkCore;
using ProductCatalogApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogApi.Data
{
    // Start Class #10: Sun 3-1-20

    public class CatalogContext : DbContext
    {
        // Dependency Injection
        public CatalogContext(DbContextOptions options) : base(options) { }
        // Passing the "WHERE" (where to create tables) on to the base
        // constructor
        // (In C#, constructors are not inherited)
        // *** If you want constructor params to base class constructor, you
        // must do so explicitly



        // Now, WHAT code should be converted to tables
        // Since CatalogContext derives from DbContext
        // EntityFramework knows to look for public properties
        // of type DbSet<T>...essentially an instruction for
        // Entity Framework
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        // A DbSet is a db table, (set is another name for table)
        // CatalogBrands will now be a database table of CatalogBrand
        // I want a table (DbSet) that is made out of this schema (CatalogBrand)

        // The name "CatalogBrands" is for your internal reference
        // Default table name in db will be plural of schema name:
        // (CatalogBrand -> CatalogBrands)

        public DbSet<CatalogType> CatalogTypes { get; set; }

        public DbSet<CatalogItem> CatalogItems { get; set; }

        // That takes care of "what"


        // HOW to configure tables

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // XX BAD NO: base.OnModelCreating(modelBuilder);
            // No. DbContext (parent class) does not know how to build my tables
            // (This is just the default behavior from VS when overriding methods)

            // Recall that "entity" is another name for table, also "set"
            // Entity common word from database world.

            // Who will create tables? modelBuilder

            // We have three entities...CatalogBrand, CatalogType and CatalogItem


            // ModelBuilder.Entity Method
            // overload: Entity<TEntity>( Action<EntityTypeBuilder<TEntity>> )
            // This overload allows configuration of the entity type to be done
            // inline in the method call rather than being chained after a call
            // to Entity<TEntity>()
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.modelbuilder.entity?view=efcore-3.1#Microsoft_EntityFrameworkCore_ModelBuilder_Entity__1_System_Action_Microsoft_EntityFrameworkCore_Metadata_Builders_EntityTypeBuilder___0___
            modelBuilder.Entity<CatalogBrand>(
                // ModelBuilder.Entity<TEntity>(Action<EntityTypeBuilder<TEntity>>)
                // Calling the templatized method Entity<TEntity>,
                // for TEntity == CatalogBrand
                // To pass in an Action<EntityTypeBuilder<TEntity>>,
                // for TEntity == CatalogBrand, so:
                // Action<EntityTypeBuilder<CatalogBrand>>
                e => // let e be that entity (more precisely, that EntityTypeBuilder<CatalogBrand>),
                // the table that is being built (the CatalogBrand one)
                // For this EntityTypeBuilder<CatalogBrand> e,
                // here is the set of instructions I provide to you to go build
                // that table:
                // That is, telling modelBuilder that these are the calls to make
                {
                    // Configures the table that the entity type maps to when
                    // targeting a relational database.
                    e.ToTable("CatalogBrands");
                    // "RelationalEntityTypeBuilderExtensions.ToTable Method"
                    // ToTable<TEntity>(EntityTypeBuilder<TEntity>, String)
                    // Or, essentially:
                    // EntityTypeBuilder<TEntity>.ToTable<TEntity>(String)
                    // This is where you can name the db table something other
                    // than the default if you want (plural of the schema/class name)

                    // QUESTION (sort-of answered above):
                    // Intellisense shows the ToTable() that takes a
                    // single string parameter. Can't find it in the docs.
                    // I don't get it.
                    // Even GoToDefinition takes me to this two-param version:
                    // ToTable<TEntity>(EntityTypeBuilder<TEntity>, String)
                    // Is the first param the invoking object since this is
                    // an "extension method?" The docs include that ToTable()
                    // returns the same (invoking?) builder instance so that
                    // multiple calls can be chained:
                    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.relationalentitytypebuilderextensions.totable?view=efcore-3.1#Microsoft_EntityFrameworkCore_RelationalEntityTypeBuilderExtensions_ToTable__1_Microsoft_EntityFrameworkCore_Metadata_Builders_EntityTypeBuilder___0__System_String_

                    // Want to make the Id column the primary column
                    // and have it autogenerate values for it

                    // Choose Property to provide instruction for
                    // XX e.Property();
                    // It's asking for a property name as a string
                    // don't like that. What if my property name changes?


                    // "Pass in a Func that picks the property for you,"
                    // a nested lambda:
                    // EntityTypeBuilder<TEntity>.Property<TProperty>(
                    //      Expression< Func <TEntity,TProperty> >)
                    // Where TEntity == CatalogBrand and TProperty == int,
                    // So: EntityTypeBuilder<CatalogBrand>.Property<int>(
                    //      Expression< Func <CatalogBrand,int> >)
                    // QUESTION: Correct?
                    e.Property(b => b.Id)
                    // Expression< Func <TEntity,TProperty> >
                    // So: Expression< Func <CatalogBrand,int> >
                    // "A lambda expression representing the property to be configured"
                    // Let b be the CatalogBrand. Then choose a property from that class
                    // that I want to provide instructions for (Id)
                    // Now let's go define some rules for this property
                    // on CatalogBrand(Id)
                    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.entitytypebuilder-1.property?view=efcore-3.1#Microsoft_EntityFrameworkCore_Metadata_Builders_EntityTypeBuilder_1_Property__1_System_Linq_Expressions_Expression_System_Func__0___0___
                    // If do it the Func way instead of by string, then compiler
                    // will catch if you forget to change it here
                    // "Can't find Id anymore"

                    // Property() returns an object that can be used to configure
                    // the property: PropertyBuilder<int> (since Id is of type int)
                    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.propertybuilder-1?view=efcore-3.1

                    // Now, continuing from e.Property(b => b.Id) above:
                        .IsRequired()
                        // First rule for the Id property
                        // you need to have an Id (that is, it is required)
                        // PropertyBuilder<int>.IsRequired()
                        // https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.propertybuilder-1.isrequired?view=efcore-3.1#Microsoft_EntityFrameworkCore_Metadata_Builders_PropertyBuilder_1_IsRequired_System_Boolean_
                        .UseHiLo("catalog_brand_hilo");
                    // Second, want it to be a primary key and autogenerated
                    // .UseHihLo() is new in 3.1
                    // Former call is deprecated: .ForSqlServerUseSequenceHiLo()
                    // Probably because of explicit mention of sql server

                    // What is HiLo?
                    // In db, primary keys assigned in a sequence
                    // Recall flow: Amazon page -> API -> database
                    // Takes time for the db to autogenerate an Id value for you
                    // But user wants to see their order Id right away

                    // OLD WAY to do: (don't do this, use HiLo)
                    // Must be split onto two lines:
                    // e.HasKey(b => b.Id);
                    // I want a primary key for my table
                    // XX e.Property(b => b.Id)
                    //  .ValueGeneratedOnAdd();
                    // Every time ... I want you to autogenerate the value for
                    // the id column
                    // This id should be generated every time I add a new row
                    // into the table

                    // For performance reasons, instead HiLo technique is recommended
                    // Hi and Lo is a range
                    // Establish connection to your database
                    // Database gives you back a range
                    // As a client, you can pick any number in this range

                    // Brillan's question: When is remainder released?
                    // When process is no longer alive. Back into the queue.

                    // In the db world, speak in terms of constraints
                    // constraints are rules
                    // Good practice to give a name for your constraint
                    // Use to refer to it for deletion, etc.
                    // If you don't name it, then no option to go back and
                    // change or delete it

                    // Note:
                    // .UseHiLo("catalog_brand_hilo");
                    // Automatically makes as primary key, as HasKey() did/does
                    // Don't need to do anything else to mark it as a key


                    // Next one:
                    e.Property(b => b.Brand)
                        .IsRequired()
                        .HasMaxLength(100);
                    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.builders.propertybuilder-1.hasmaxlength?view=efcore-3.1#Microsoft_EntityFrameworkCore_Metadata_Builders_PropertyBuilder_1_HasMaxLength_System_Int32_
                    // Control how big your character field is

                    // Recall SQL is not SQL Server
                    // SQL is just Structured Query Language
                    // commonly used in relational db world

                    // Of note: C# data types may or may not map directly to the
                    // SQL data types
                    // int -> int
                    // bool -> bit
                    // string -> Varchar
                    // string -> char

                    // In SQL:
                    // Varchar(100) == variable charcter
                    // what is variable is the size in memory
                    // only allocates what you use (out of total possible)

                    // char(100) 
                    // char is fixed in size
                    // always allocates all 100
                    // if you only use 20, the rest of the 80 will be spaces
                    // char good for things like zip code

                    // Sounds like HasMaxLength implies Varchar
                });
            // That is all instructions going to give for Brand table
            // done.

            // See how nice it is to write a lambda instead of another method?

            modelBuilder.Entity<CatalogType>(e =>
                {
                    e.ToTable("CatalogTypes");
                    e.Property(t => t.Id)
                        .IsRequired()
                        .UseHiLo("catalog_type_hilo"); // Name the constraint
                    e.Property(t => t.Type)
                        .IsRequired()
                        .HasMaxLength(100);
                });

            // The "HOW" instructions for how to build the tables for
            // CatalogType and CatalogBrand are similar

            modelBuilder.Entity<CatalogItem>(e =>
                {
                    e.ToTable("Catalog"); // Not "CatalogItems"
                    // No access to this name in C#.
                    // Internal to the EF to name the table
                    e.Property(c => c.Id)
                        .IsRequired()
                        .UseHiLo("catalog_hilo");
                    e.Property(c => c.Name)
                        .IsRequired()
                        .HasMaxLength(100);
                    // For strings (Varchar) DO set a max length
                    // Otherwise will be set to the big max
                    // someone might try to throw an entire file in there
                    // End up with a bad kind of injection (SQL injection)
                    e.Property(c => c.Price)
                        .IsRequired();
                    // Price is a decimal column. Length isn't relevant.



                    // FOREIGN KEY RELATIONSHIP:

                    // Now comes foreign key relationship

                    // Three kinds of relationships in the db world:
                    // 1:1
                    // 1:M / M:1
                    // M:M

                    // Let's figure out relationship between CatalogType
                    // and CatalogItem:

                    // 1:1
                    // For one CatalogType, there can be only one CatalogItem
                    // But that doesn't make any sense

                    // From the CatalogItem side, if you are looking at a specific
                    // jewelry, it is associated with one, and only one type
                    // That does make sense.

                    // For 1:1, have to look at it from both ends
                    // (to see if it makes sense)

                    // 1:1 does not work, let's look at 1:M
                    // A ring (CatalogType) can have many CatalogItem
                    // but a CatalogItem is associated with only one type
                    // that makes sense.
                    // So, could be 1:M

                    // M:M usually means
                    // CatalogType can have many CatalogItem
                    // and CatalogItem can have many CatalogType
                    // M:M requires a third table
                    // Third table contains rows of CT id and C id
                    // (No direct relationship between the two tables)
                    // Means that you would have to write a third class
                    // with those two Ids in place


                    // But in our case, the relationship is 1:M
                    // Recall from CatalogItem class, have:
                    // int CatalogTypeId
                    // CatalogType CatalogType

                    // CatalogItem has one relationship to CatalogType
                    // CatalogType, in turn, has many relationship to CatalogItem
                    // How to write that?
                    // Recall that e is EntityTypeBuilder<CatalogItem>
                    // Looking at this from the perspective of CatalogItem

                    // Each CatalogItem has only one type
                    e.HasOne(c => c.CatalogType) // CatalogItem -> CatalogType 1:M
                    // Note that this is the relationship with the table
                    // not with the id (CatalogTypeId)
                    // This is what the virtual navigational property is for

                    // This entity, the CatalogItem, has one relationship
                    // to the CatalogType table.
                    // Then, that table, in turn, has many relationships to the
                    // CatalogItem table
                        .WithMany() // CatalogType -> CatalogItem M:1
                    // 
                        .HasForeignKey(c => c.CatalogTypeId);
                    // Used two properties in this relationship
                    // CatalogTypeId is now officially the foreign key of this
                    // relationshp, and relating CatalogItem-self to the
                    // CatalogType table

                    // Relationship maintained through foreign key CatalogTypeId

                    // If you neglect to specify a foreign key property
                    // EF will create a foreign key column (for a catalog type id)
                    // that you have no access to
                    // Then you wouldn't be able to "show me all of the items that
                    // are related to catalog id type 1" (do a query on it)


                    // DO:
                    // Explicitly create a catalog type id and make it as foreign key

                    e.HasOne(c => c.CatalogBrand) // Again, the table not the id
                                                  // Relating to a specific table
                                                  // not a column within that table (? primary key?)
                        .WithMany()
                        .HasForeignKey(c => c.CatalogBrandId);

                    // Recall that relationships are defined at the schema level

                    // QUESTION:
                    // ? What about Description and PictureUrl

                    //e.Property(c => c.Description)
                    //    .IsRequired()
                    //    .HasMaxLength(100);

                });
            // Now done configuring Entity Framework,
            // Done with the WHERE, WHAT and HOW of the database

            // Now it's about how to make it possible to see it
            // Now is time for injection
            // Who will tell me where

            // Go to Startup.cs file...
            // https://youtu.be/1k11n8FO_UU?list=PLdbymrfiqF-wmh3VsbxysBsu2O9w6Z3Ks&t=3232
        }

    }
}
