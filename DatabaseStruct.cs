using TT_DBLib.Storage;

namespace MuscicCenter.Storage
{
    public abstract class DatabaseStruct
    {
        public static string AccountTable = "Account";
        public static string ArtistTable = "Artist";
        public static string CategoryTable = "Category";
        public static string RecordTable = "Record";
        public static string CommentsTable = "Comments";
        public static string FarvouriteTable = "Favourite";
        public static string InfoPlaylistTable = "InfoPlaylist";
        public static string PlaylistTable = "Playlist";
        public static string ViewsHistoryTable = "ViewHistory";
        public static string ArtistVisitHistoryTable = "ArtistVisitHistory";
        public static string ReplyCommentTable = "ReplyComment";
        public static string OnlinesTable = "Onlines";
        public static string ReactCommentTable = "ReactComment";

        public List<TableInfo> getTables()
        {
            return new List<TableInfo>
        {
            new TableInfo
            {
                TableName = AccountTable,
                key = "UserName",
                cauTrucDuLieu = @"
                    UserName NVARCHAR(20) PRIMARY KEY,
                    Password VARCHAR(MAX),
                    Status NVARCHAR(50),
                    JoinDay DATETIME DEFAULT GETDATE()"
            },
            new TableInfo
            {
                TableName = ArtistTable,
                key = "ArtistUid",
                cauTrucDuLieu = $@"
                    ArtistUid NVARCHAR(50) PRIMARY KEY,
                    StageName NVARCHAR(max),
                    Avata NVARCHAR(MAX),
                    Visits INT"
            },
            new TableInfo
            {
                TableName = CategoryTable,
                key = "CategoryUid",
                cauTrucDuLieu = $@"
                    CategoryUid NVARCHAR(50) PRIMARY KEY,
                    DisplayName NVARCHAR(MAX)"
            },
            new TableInfo
            {
                TableName = RecordTable,
                key = "RecordUid",
                cauTrucDuLieu = $@"
                    RecordUid NVARCHAR(50) PRIMARY KEY,
                    CategoryUid NVARCHAR(50) FOREIGN KEY REFERENCES {CategoryTable}(CategoryUid),
                    ArtistUid NVARCHAR(50) FOREIGN KEY REFERENCES {ArtistTable}(ArtistUid),
                    Record NVARCHAR(MAX),
                    CoverPhoto NVARCHAR(MAX),
                    Poster NVARCHAR(MAX),
                    Lyrics NVARCHAR(MAX),
                    Description NVARCHAR(MAX),
                    Views INT"
            },
            new TableInfo
            {
                TableName = CommentsTable,
                key = "CommentUid",
                cauTrucDuLieu = $@"
                    CommentUid NVARCHAR(50) PRIMARY KEY,
                    UserName NVARCHAR(20) FOREIGN KEY REFERENCES {AccountTable}(UserName),
                    RecordUid NVARCHAR(50) FOREIGN KEY REFERENCES {RecordTable}(RecordUid),
                    Content NVARCHAR(MAX),
                    TimeOfComment DATETIME DEFAULT GETDATE()"
            },new TableInfo
            {
                TableName = PlaylistTable,
                key = "PlaylistUid",
                cauTrucDuLieu = $@"
                    PlaylistUid NVARCHAR(50) PRIMARY KEY,
                    UserName NVARCHAR(20) FOREIGN KEY REFERENCES {AccountTable}(UserName),
                    DisplayName NVARCHAR(MAX)"
            },new TableInfo
            {
                TableName = InfoPlaylistTable,
                key = "PlaylistUid",
                cauTrucDuLieu = $@"
                    PlaylistUid NVARCHAR(20) FOREIGN KEY REFERENCES {PlaylistTable}(PlaylistUid),
                    RecordUid NVARCHAR(50) FOREIGN KEY REFERENCES {RecordTable}(RecordUid)"
            },new TableInfo
            {
                TableName = FarvouriteTable,
                key = "UserName",
                cauTrucDuLieu = $@"
                    UserName NVARCHAR(20) FOREIGN KEY REFERENCES {AccountTable}(UserName),
                    RecordUid NVARCHAR(50) FOREIGN KEY REFERENCES {RecordTable}(RecordUid),
                    DayOfSet DATETIME DEFAULT GETDATE()"
            },
        };
        }
    }
}