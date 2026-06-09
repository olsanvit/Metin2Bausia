-- ============================================================
-- Metin2Bausia — PostgreSQL management database
-- Tabulky pro agenty, approval workflow, content management
-- ============================================================

-- Rozšíření
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
CREATE EXTENSION IF NOT EXISTS "citext";

-- ── Items ─────────────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS "Items" (
    "Guid"              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"         timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"         timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"         boolean NOT NULL DEFAULT false,
    "Emoji"             text NOT NULL DEFAULT '',
    "Colors"            text NOT NULL DEFAULT '',

    -- Metin2 identity
    "Vnum"              integer UNIQUE,                  -- item číslo (v item_proto)
    "Name"              text NOT NULL DEFAULT '',
    "NormalizedName"    citext,
    "LocaleName"        text,                            -- zobrazovaný název
    "ItemType"          text,                            -- weapon, armor, belt, ...
    "SubType"           integer,
    "Weight"            integer,
    "Size"              integer,
    "AntiFlags"         bigint DEFAULT 0,
    "Flags"             bigint DEFAULT 0,
    "WearFlags"         bigint DEFAULT 0,
    "ImmuneFlags"       bigint DEFAULT 0,
    "Gold"              bigint DEFAULT 0,
    "Buy"               bigint DEFAULT 0,
    "LimitType0"        text, "LimitValue0" integer,
    "LimitType1"        text, "LimitValue1" integer,
    "ApplyType0"        text, "ApplyValue0" integer,
    "ApplyType1"        text, "ApplyValue1" integer,
    "ApplyType2"        text, "ApplyValue2" integer,
    "Value0"            bigint DEFAULT 0,
    "Value1"            bigint DEFAULT 0,
    "Value2"            bigint DEFAULT 0,
    "Value3"            bigint DEFAULT 0,
    "Value4"            bigint DEFAULT 0,
    "Value5"            bigint DEFAULT 0,
    "IconFile"          text,
    "ModelFile"         text,
    "Description"       text,
    "SummonMobVnum"     integer,

    -- Approval workflow
    "ContentStatus"     text NOT NULL DEFAULT 'pending',   -- pending|approved|rejected|needs_review
    "ApprovedAt"        timestamptz,
    "ApprovedBy"        text,
    "RejectedAt"        timestamptz,
    "RejectedReason"    text,

    -- Agent provenance
    "SourceName"        text,
    "SourceUrl"         text,
    "SourceDate"        date,
    "DataOrigin"        text DEFAULT 'ai_collected',    -- ai_collected|manual|imported
    "VerifiedAt"        timestamptz,

    -- Scoring
    "FinalScore"        numeric(6,3),
    "ConfidenceScore"   numeric(6,3),
    "QualityScore"      numeric(6,3),
    "DataScore"         numeric(6,3),
    "ReliabilityScore"  numeric(6,3),
    "ValidationScore"   numeric(6,3),
    "CompletenessScore" numeric(6,3),
    "PopularityScore"   numeric(6,3),
    "PriorityScore"     numeric(6,3),
    "ImportanceScore"   numeric(6,3),
    "FreshnessScore"    numeric(6,3),
    "ConsistencyScore"  numeric(6,3),
    "AccuracyScore"     numeric(6,3),
    "AiScore"           numeric(6,3),
    "AnomalyScore"      numeric(6,3),
    "RiskScore"         numeric(6,3),
    "TrendScore"        numeric(6,3),
    "ActivityScore"     numeric(6,3),
    "EngagementScore"   numeric(6,3),
    "HistoricalScore"   numeric(6,3),
    "CoverageScore"     numeric(6,3),
    "SourceScore"       numeric(6,3),

    "Metadata"          jsonb DEFAULT '{}'
);
CREATE INDEX IF NOT EXISTS idx_items_vnum ON "Items"("Vnum");
CREATE INDEX IF NOT EXISTS idx_items_status ON "Items"("ContentStatus");
CREATE INDEX IF NOT EXISTS idx_items_normalized ON "Items"("NormalizedName");

-- ── Mobs (Monsters) ───────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS "Mobs" (
    "Guid"              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"         timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"         timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"         boolean NOT NULL DEFAULT false,
    "Emoji"             text NOT NULL DEFAULT '',
    "Colors"            text NOT NULL DEFAULT '',

    "Vnum"              integer UNIQUE,
    "Name"              text NOT NULL DEFAULT '',
    "NormalizedName"    citext,
    "LocaleName"        text,
    "MobType"           text,                            -- monster|npc|boss|...
    "Rank"              text,                            -- pawn|knight|boss|king
    "BattleType"        text,
    "Level"             integer,
    "ScalePercent"      integer DEFAULT 100,
    "GoldMin"           bigint DEFAULT 0,
    "GoldMax"           bigint DEFAULT 0,
    "Exp"               bigint DEFAULT 0,
    "MaxHp"             bigint DEFAULT 0,
    "RegenCycle"        integer DEFAULT 0,
    "RegenPercent"      integer DEFAULT 0,
    "GoldDropRate"      integer DEFAULT 0,
    "AttackSpeed"       integer DEFAULT 0,
    "MoveSpeed"         integer DEFAULT 0,
    "AggressiveHpPct"   integer DEFAULT 0,
    "AggressiveSight"   integer DEFAULT 0,
    "AttackRange"       integer DEFAULT 0,
    "Size"              integer DEFAULT 0,
    "Atk"               integer DEFAULT 0,
    "MagicAtk"          integer DEFAULT 0,
    "Def"               integer DEFAULT 0,
    "MagicDef"          integer DEFAULT 0,
    "ImmuneFlags"       bigint DEFAULT 0,
    "Resists"           jsonb DEFAULT '{}',
    "DropItems"         jsonb DEFAULT '[]',              -- [{vnum, count, pct}, ...]
    "IconFile"          text,
    "ModelFile"         text,
    "Description"       text,

    -- Approval
    "ContentStatus"     text NOT NULL DEFAULT 'pending',
    "ApprovedAt"        timestamptz,
    "ApprovedBy"        text,
    "RejectedAt"        timestamptz,
    "RejectedReason"    text,

    -- Agent provenance
    "SourceName"        text,
    "SourceUrl"         text,
    "SourceDate"        date,
    "DataOrigin"        text DEFAULT 'ai_collected',
    "VerifiedAt"        timestamptz,

    "FinalScore" numeric(6,3), "ConfidenceScore" numeric(6,3), "QualityScore" numeric(6,3),
    "DataScore" numeric(6,3), "ReliabilityScore" numeric(6,3), "ValidationScore" numeric(6,3),
    "CompletenessScore" numeric(6,3), "PopularityScore" numeric(6,3), "PriorityScore" numeric(6,3),
    "ImportanceScore" numeric(6,3), "FreshnessScore" numeric(6,3), "ConsistencyScore" numeric(6,3),
    "AccuracyScore" numeric(6,3), "AiScore" numeric(6,3), "AnomalyScore" numeric(6,3),
    "RiskScore" numeric(6,3), "TrendScore" numeric(6,3), "ActivityScore" numeric(6,3),
    "EngagementScore" numeric(6,3), "HistoricalScore" numeric(6,3), "CoverageScore" numeric(6,3),
    "SourceScore" numeric(6,3),
    "Metadata"          jsonb DEFAULT '{}'
);
CREATE INDEX IF NOT EXISTS idx_mobs_vnum ON "Mobs"("Vnum");
CREATE INDEX IF NOT EXISTS idx_mobs_status ON "Mobs"("ContentStatus");

-- ── Maps ──────────────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS "Maps" (
    "Guid"              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"         timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"         timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"         boolean NOT NULL DEFAULT false,
    "Emoji"             text NOT NULL DEFAULT '',
    "Colors"            text NOT NULL DEFAULT '',

    "MapIndex"          integer UNIQUE,
    "Name"              text NOT NULL DEFAULT '',
    "NormalizedName"    citext,
    "LocaleName"        text,
    "MapType"           text,                            -- field|dungeon|pvp|...
    "Width"             integer,
    "Height"            integer,
    "BaseX"             integer,
    "BaseY"             integer,
    "CellScale"         integer DEFAULT 25,
    "MinLevel"          integer DEFAULT 0,
    "MaxLevel"          integer DEFAULT 0,
    "SpawnMobs"         jsonb DEFAULT '[]',              -- [{vnum, count, ...}, ...]
    "MapFiles"          jsonb DEFAULT '{}',              -- {attr, setting, terrain, ...}
    "Description"       text,
    "PreviewImageUrl"   text,

    "ContentStatus"     text NOT NULL DEFAULT 'pending',
    "ApprovedAt"        timestamptz,
    "ApprovedBy"        text,
    "RejectedAt"        timestamptz,
    "RejectedReason"    text,

    "SourceName"        text, "SourceUrl"  text,
    "SourceDate"        date, "DataOrigin" text DEFAULT 'ai_collected',
    "VerifiedAt"        timestamptz,

    "FinalScore" numeric(6,3), "ConfidenceScore" numeric(6,3), "QualityScore" numeric(6,3),
    "DataScore" numeric(6,3), "ReliabilityScore" numeric(6,3), "ValidationScore" numeric(6,3),
    "CompletenessScore" numeric(6,3), "PopularityScore" numeric(6,3), "PriorityScore" numeric(6,3),
    "ImportanceScore" numeric(6,3), "FreshnessScore" numeric(6,3), "ConsistencyScore" numeric(6,3),
    "AccuracyScore" numeric(6,3), "AiScore" numeric(6,3), "AnomalyScore" numeric(6,3),
    "RiskScore" numeric(6,3), "TrendScore" numeric(6,3), "ActivityScore" numeric(6,3),
    "EngagementScore" numeric(6,3), "HistoricalScore" numeric(6,3), "CoverageScore" numeric(6,3),
    "SourceScore" numeric(6,3),
    "Metadata"          jsonb DEFAULT '{}'
);

-- ── Skills ────────────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS "Skills" (
    "Guid"              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"         timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"         timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"         boolean NOT NULL DEFAULT false,
    "Emoji"             text NOT NULL DEFAULT '',
    "Colors"            text NOT NULL DEFAULT '',

    "Vnum"              integer UNIQUE,
    "Name"              text NOT NULL DEFAULT '',
    "NormalizedName"    citext,
    "LocaleName"        text,
    "SkillType"         text,                            -- active|passive|horse|...
    "JobFlags"          bigint DEFAULT 0,
    "TargetType"        integer DEFAULT 0,
    "MaxLevel"          integer DEFAULT 20,
    "Description"       text,
    "ScriptFile"        text,
    "IconFile"          text,

    "ContentStatus"     text NOT NULL DEFAULT 'pending',
    "ApprovedAt"        timestamptz, "ApprovedBy" text,
    "RejectedAt"        timestamptz, "RejectedReason" text,

    "SourceName"        text, "SourceUrl" text, "SourceDate" date,
    "DataOrigin"        text DEFAULT 'ai_collected', "VerifiedAt" timestamptz,

    "FinalScore" numeric(6,3), "ConfidenceScore" numeric(6,3), "QualityScore" numeric(6,3),
    "DataScore" numeric(6,3), "ReliabilityScore" numeric(6,3), "ValidationScore" numeric(6,3),
    "CompletenessScore" numeric(6,3), "AiScore" numeric(6,3), "RiskScore" numeric(6,3),
    "PopularityScore" numeric(6,3), "PriorityScore" numeric(6,3), "ImportanceScore" numeric(6,3),
    "FreshnessScore" numeric(6,3), "AccuracyScore" numeric(6,3), "AnomalyScore" numeric(6,3),
    "EngagementScore" numeric(6,3), "HistoricalScore" numeric(6,3), "CoverageScore" numeric(6,3),
    "ConsistencyScore" numeric(6,3), "TrendScore" numeric(6,3), "ActivityScore" numeric(6,3),
    "SourceScore" numeric(6,3),
    "Metadata"          jsonb DEFAULT '{}'
);

-- ── Quests ────────────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS "Quests" (
    "Guid"              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"         timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"         timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"         boolean NOT NULL DEFAULT false,
    "Emoji"             text NOT NULL DEFAULT '',
    "Colors"            text NOT NULL DEFAULT '',

    "QuestName"         text NOT NULL DEFAULT '',
    "NormalizedName"    citext,
    "LocaleName"        text,
    "QuestType"         text,                            -- main|side|repeatable|daily|...
    "MinLevel"          integer DEFAULT 0,
    "MaxLevel"          integer DEFAULT 0,
    "StartNpcVnum"      integer,
    "LuaScript"         text,                            -- quest Lua script content
    "Description"       text,
    "Rewards"           jsonb DEFAULT '[]',

    "ContentStatus"     text NOT NULL DEFAULT 'pending',
    "ApprovedAt"        timestamptz, "ApprovedBy" text,
    "RejectedAt"        timestamptz, "RejectedReason" text,

    "SourceName"        text, "SourceUrl" text, "SourceDate" date,
    "DataOrigin"        text DEFAULT 'ai_collected', "VerifiedAt" timestamptz,

    "FinalScore" numeric(6,3), "ConfidenceScore" numeric(6,3), "QualityScore" numeric(6,3),
    "DataScore" numeric(6,3), "ReliabilityScore" numeric(6,3), "ValidationScore" numeric(6,3),
    "CompletenessScore" numeric(6,3), "AiScore" numeric(6,3), "RiskScore" numeric(6,3),
    "PopularityScore" numeric(6,3), "PriorityScore" numeric(6,3), "ImportanceScore" numeric(6,3),
    "FreshnessScore" numeric(6,3), "AccuracyScore" numeric(6,3), "AnomalyScore" numeric(6,3),
    "EngagementScore" numeric(6,3), "HistoricalScore" numeric(6,3), "CoverageScore" numeric(6,3),
    "ConsistencyScore" numeric(6,3), "TrendScore" numeric(6,3), "ActivityScore" numeric(6,3),
    "SourceScore" numeric(6,3),
    "Metadata"          jsonb DEFAULT '{}'
);

-- ── Systems ───────────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS "Systems" (
    "Guid"              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"         timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"         timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"         boolean NOT NULL DEFAULT false,
    "Emoji"             text NOT NULL DEFAULT '',
    "Colors"            text NOT NULL DEFAULT '',

    "SystemName"        text NOT NULL DEFAULT '',
    "NormalizedName"    citext,
    "Category"          text,                            -- crafting|enchant|mount|pet|pvp|...
    "Description"       text,
    "Implementation"    text,                            -- jak implementovat (instrukce)
    "Files"             jsonb DEFAULT '[]',              -- seznam souborů ke změně
    "Complexity"        text,                            -- low|medium|high
    "SourceServer"      text,                            -- odkud systém pochází

    "ContentStatus"     text NOT NULL DEFAULT 'pending',
    "ApprovedAt"        timestamptz, "ApprovedBy" text,
    "RejectedAt"        timestamptz, "RejectedReason" text,

    "SourceName"        text, "SourceUrl" text, "SourceDate" date,
    "DataOrigin"        text DEFAULT 'ai_collected', "VerifiedAt" timestamptz,

    "FinalScore" numeric(6,3), "ConfidenceScore" numeric(6,3), "QualityScore" numeric(6,3),
    "DataScore" numeric(6,3), "ReliabilityScore" numeric(6,3), "ValidationScore" numeric(6,3),
    "CompletenessScore" numeric(6,3), "AiScore" numeric(6,3), "RiskScore" numeric(6,3),
    "PopularityScore" numeric(6,3), "PriorityScore" numeric(6,3), "ImportanceScore" numeric(6,3),
    "FreshnessScore" numeric(6,3), "AccuracyScore" numeric(6,3), "AnomalyScore" numeric(6,3),
    "EngagementScore" numeric(6,3), "HistoricalScore" numeric(6,3), "CoverageScore" numeric(6,3),
    "ConsistencyScore" numeric(6,3), "TrendScore" numeric(6,3), "ActivityScore" numeric(6,3),
    "SourceScore" numeric(6,3),
    "Metadata"          jsonb DEFAULT '{}'
);

-- ── ContentImages ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS "ContentImages" (
    "Guid"              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"         timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"         timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"         boolean NOT NULL DEFAULT false,
    "Emoji"             text NOT NULL DEFAULT '',
    "Colors"            text NOT NULL DEFAULT '',

    "EntityTable"       text NOT NULL,                  -- Items|Mobs|Maps|...
    "EntityGuid"        uuid,
    "EntityVnum"        integer,
    "ImageType"         text,                            -- icon|preview|texture|...
    "OriginalUrl"       text,
    "LocalPath"         text,                            -- uložená cesta na QNAP
    "PublicUrl"         text,                            -- veřejná URL přes MCP
    "FileSize"          bigint,
    "MimeType"          text,
    "Sha256"            text UNIQUE,
    "Width"             integer,
    "Height"            integer,
    "SourceName"        text,
    "SourceUrl"         text,

    "ContentStatus"     text NOT NULL DEFAULT 'pending',
    "ApprovedAt"        timestamptz, "ApprovedBy" text,

    "FinalScore" numeric(6,3), "ConfidenceScore" numeric(6,3), "QualityScore" numeric(6,3),
    "DataScore" numeric(6,3), "ReliabilityScore" numeric(6,3), "AiScore" numeric(6,3),
    "RiskScore" numeric(6,3), "AccuracyScore" numeric(6,3), "FreshnessScore" numeric(6,3),
    "CompletenessScore" numeric(6,3), "PopularityScore" numeric(6,3), "PriorityScore" numeric(6,3),
    "ImportanceScore" numeric(6,3), "ValidationScore" numeric(6,3), "ConsistencyScore" numeric(6,3),
    "AnomalyScore" numeric(6,3), "TrendScore" numeric(6,3), "ActivityScore" numeric(6,3),
    "EngagementScore" numeric(6,3), "HistoricalScore" numeric(6,3), "CoverageScore" numeric(6,3),
    "SourceScore" numeric(6,3),
    "Metadata"          jsonb DEFAULT '{}'
);
CREATE INDEX IF NOT EXISTS idx_images_entity ON "ContentImages"("EntityTable","EntityGuid");
CREATE INDEX IF NOT EXISTS idx_images_sha256 ON "ContentImages"("Sha256");
