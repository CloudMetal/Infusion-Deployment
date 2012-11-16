using System;
using System.Collections.Generic;
using System.Data;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Infusion.Documents {
    public class Migrations : DataMigrationImpl {

        public int Create() {
			// Creating table DocumentPartRecord
            SchemaBuilder.CreateTable("DocumentPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("Description", c => c.Unlimited())
                );

            SchemaBuilder.CreateTable("DocumentContentPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<int>("SourceDocumentId")
                );

            SchemaBuilder.CreateTable("DocumentDocumentContentsRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("DocumentPartRecord_Id")
                    .Column<int>("DocumentContentPartRecord_Id")
                );

            ContentDefinitionManager.AlterTypeDefinition("Document",
                cfg => cfg
                    .WithPart("DocumentPart")
                    .WithPart("CommonPart")
                    .WithPart("TitlePart")
                    .WithPart("AutoroutePart", builder => builder
                        .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                        .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                        .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Title', Pattern: '{Content.Slug}', Description: 'my-blog'}]")
                        .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                    .WithPart("MenuPart")
                    .WithPart("AdminMenuPart", p => p.WithSetting("AdminMenuPartTypeSettings.DefaultPosition", "2"))
                );

            ContentDefinitionManager.AlterTypeDefinition("DocumentContent",
                cfg => cfg
                    .WithPart("DocumentContentPart")
                    .WithPart("CommonPart", p => p
                        .WithSetting("DateEditorSettings.ShowDateEditor", "true"))
                    .WithPart("PublishLaterPart")
                    .WithPart("TitlePart")
                    .WithPart("AutoroutePart", builder => builder
                        .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                        .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                        .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Blog and Title', Pattern: '{Content.Container.Path}/{Content.Slug}', Description: 'my-blog/my-post'}]")
                        .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                    .WithPart("BodyPart")
                    .Draftable()
                );

            return 1;
        }
    }
}