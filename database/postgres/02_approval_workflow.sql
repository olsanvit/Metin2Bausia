-- ============================================================
-- Approval Workflow + Agent governance tabulky
-- ============================================================

-- ── ApprovalLog ───────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS "ApprovalLog" (
    "Guid"          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"     timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"     timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"     boolean NOT NULL DEFAULT false,
    "Emoji"         text NOT NULL DEFAULT '',
    "Colors"        text NOT NULL DEFAULT '',

    "EntityTable"   text NOT NULL,      -- Items|Mobs|Maps|Skills|Quests|Systems
    "EntityGuid"    uuid NOT NULL,
    "EntityVnum"    integer,
    "EntityName"    text,
    "Action"        text NOT NULL,      -- approved|rejected|needs_review|reverted
    "Operator"      text,
    "Reason"        text,
    "PrevStatus"    text,
    "NewStatus"     text,

    "FinalScore" numeric(6,3), "ConfidenceScore" numeric(6,3), "QualityScore" numeric(6,3),
    "DataScore" numeric(6,3), "ReliabilityScore" numeric(6,3), "ValidationScore" numeric(6,3),
    "CompletenessScore" numeric(6,3), "AiScore" numeric(6,3), "RiskScore" numeric(6,3),
    "PopularityScore" numeric(6,3), "PriorityScore" numeric(6,3), "ImportanceScore" numeric(6,3),
    "FreshnessScore" numeric(6,3), "AccuracyScore" numeric(6,3), "AnomalyScore" numeric(6,3),
    "EngagementScore" numeric(6,3), "HistoricalScore" numeric(6,3), "CoverageScore" numeric(6,3),
    "ConsistencyScore" numeric(6,3), "TrendScore" numeric(6,3), "ActivityScore" numeric(6,3),
    "SourceScore" numeric(6,3),
    "Metadata"      jsonb DEFAULT '{}'
);
CREATE INDEX IF NOT EXISTS idx_approval_entity ON "ApprovalLog"("EntityTable","EntityGuid");

-- ── ProtoFiles — sledování verzí item_proto, mob_proto, skill_proto ───────────
CREATE TABLE IF NOT EXISTS "ProtoFiles" (
    "Guid"          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"     timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"     timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"     boolean NOT NULL DEFAULT false,
    "Emoji"         text NOT NULL DEFAULT '',
    "Colors"        text NOT NULL DEFAULT '',

    "ProtoType"     text NOT NULL,          -- item_proto|mob_proto|skill_proto|...
    "Version"       integer NOT NULL,
    "FilePath"      text,                   -- cesta k souboru na serveru
    "FileSize"      bigint,
    "Sha256"        text,
    "EntryCount"    integer,
    "BuildStatus"   text DEFAULT 'draft',   -- draft|built|deployed|archived
    "DeployedAt"    timestamptz,
    "Notes"         text,
    "DiffFromPrev"  jsonb DEFAULT '{}',     -- co se změnilo oproti předchozí verzi
    "ApprovedVnums" integer[],              -- které vnum byly schváleny v této verzi

    "FinalScore" numeric(6,3), "ConfidenceScore" numeric(6,3), "QualityScore" numeric(6,3),
    "DataScore" numeric(6,3), "ReliabilityScore" numeric(6,3), "ValidationScore" numeric(6,3),
    "CompletenessScore" numeric(6,3), "AiScore" numeric(6,3), "RiskScore" numeric(6,3),
    "PopularityScore" numeric(6,3), "PriorityScore" numeric(6,3), "ImportanceScore" numeric(6,3),
    "FreshnessScore" numeric(6,3), "AccuracyScore" numeric(6,3), "AnomalyScore" numeric(6,3),
    "EngagementScore" numeric(6,3), "HistoricalScore" numeric(6,3), "CoverageScore" numeric(6,3),
    "ConsistencyScore" numeric(6,3), "TrendScore" numeric(6,3), "ActivityScore" numeric(6,3),
    "SourceScore" numeric(6,3),
    "Metadata"      jsonb DEFAULT '{}'
);

-- ── ServerConfig — konfigurace herního serveru ────────────────────────────────
CREATE TABLE IF NOT EXISTS "ServerConfig" (
    "Guid"          uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"     timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"     timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"     boolean NOT NULL DEFAULT false,
    "Emoji"         text NOT NULL DEFAULT '',
    "Colors"        text NOT NULL DEFAULT '',

    "ConfigKey"     text UNIQUE NOT NULL,
    "ConfigValue"   text,
    "Category"      text,                   -- rates|pvp|systems|features|...
    "Description"   text,
    "IsActive"      boolean DEFAULT true,
    "ChangedBy"     text,
    "PrevValue"     text,

    "FinalScore" numeric(6,3), "ConfidenceScore" numeric(6,3), "QualityScore" numeric(6,3),
    "DataScore" numeric(6,3), "ReliabilityScore" numeric(6,3), "ValidationScore" numeric(6,3),
    "CompletenessScore" numeric(6,3), "AiScore" numeric(6,3), "RiskScore" numeric(6,3),
    "PopularityScore" numeric(6,3), "PriorityScore" numeric(6,3), "ImportanceScore" numeric(6,3),
    "FreshnessScore" numeric(6,3), "AccuracyScore" numeric(6,3), "AnomalyScore" numeric(6,3),
    "EngagementScore" numeric(6,3), "HistoricalScore" numeric(6,3), "CoverageScore" numeric(6,3),
    "ConsistencyScore" numeric(6,3), "TrendScore" numeric(6,3), "ActivityScore" numeric(6,3),
    "SourceScore" numeric(6,3),
    "Metadata"      jsonb DEFAULT '{}'
);

-- Výchozí config hodnoty
INSERT INTO "ServerConfig" ("ConfigKey","ConfigValue","Category","Description") VALUES
  ('exp_rate',           '1',         'rates',    'EXP rate multiplier'),
  ('drop_rate',          '1',         'rates',    'Drop rate multiplier'),
  ('gold_rate',          '1',         'rates',    'Gold drop rate'),
  ('yang_rate',          '1',         'rates',    'Yang rate'),
  ('pvp_enabled',        'true',      'pvp',      'PvP enabled globally'),
  ('max_level',          '120',       'game',     'Maximum character level'),
  ('max_skill_level',    '20',        'game',     'Maximum skill level'),
  ('server_name',        'Bausia',    'game',     'Server display name')
ON CONFLICT ("ConfigKey") DO NOTHING;

-- ── Governance tabulky (standard) ─────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS "AgentRunReports" (
    "Guid"              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"         timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"         timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"         boolean NOT NULL DEFAULT false,
    "Emoji"             text NOT NULL DEFAULT '',
    "Colors"            text NOT NULL DEFAULT '',

    "AgentName"         text,
    "RunMode"           text,
    "Success"           boolean,
    "PromptVersion"     text,
    "SkillsVersion"     text,
    "McpVersion"        text,
    "DbStatus"          text,
    "ReadinessStatus"   text,
    "PromptStatus"      text,
    "DurationMs"        integer,
    "EntitiesProcessed" integer DEFAULT 0,
    "EntitiesInserted"  integer DEFAULT 0,
    "EntitiesUpdated"   integer DEFAULT 0,
    "EntitiesFailed"    integer DEFAULT 0,
    "BlockerCategory"   text,
    "Highlights"        jsonb DEFAULT '[]',
    "Errors"            jsonb DEFAULT '[]',
    "Notes"             text,
    "Metadata"          jsonb DEFAULT '{}',

    "FinalScore" numeric(6,3), "ConfidenceScore" numeric(6,3), "QualityScore" numeric(6,3),
    "DataScore" numeric(6,3), "ReliabilityScore" numeric(6,3), "ValidationScore" numeric(6,3),
    "CompletenessScore" numeric(6,3), "AiScore" numeric(6,3), "RiskScore" numeric(6,3),
    "PopularityScore" numeric(6,3), "PriorityScore" numeric(6,3), "ImportanceScore" numeric(6,3),
    "FreshnessScore" numeric(6,3), "AccuracyScore" numeric(6,3), "AnomalyScore" numeric(6,3),
    "EngagementScore" numeric(6,3), "HistoricalScore" numeric(6,3), "CoverageScore" numeric(6,3),
    "ConsistencyScore" numeric(6,3), "TrendScore" numeric(6,3), "ActivityScore" numeric(6,3),
    "SourceScore" numeric(6,3)
);

CREATE TABLE IF NOT EXISTS "AgentSchedules" (
    "Guid"              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"         timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"         timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"         boolean NOT NULL DEFAULT false,
    "Emoji"             text NOT NULL DEFAULT '',
    "Colors"            text NOT NULL DEFAULT '',
    "AgentName"         text UNIQUE,
    "AgentType"         text,
    "ScheduledRunTime"  time,
    "IntervalHours"     integer DEFAULT 24,
    "Timezone"          text DEFAULT 'Europe/Prague',
    "LastRunAt"         timestamptz,
    "NextRunAt"         timestamptz,
    "IsActive"          boolean DEFAULT true,
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

CREATE TABLE IF NOT EXISTS "AuditLog" (
    "Guid"              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"         timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"         timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"         boolean NOT NULL DEFAULT false,
    "Emoji"             text NOT NULL DEFAULT '',
    "Colors"            text NOT NULL DEFAULT '',
    "Action"            text,
    "TableName"         text,
    "RecordGuid"        uuid,
    "AgentName"         text,
    "IdempotencyKey"    text,
    "Data"              jsonb DEFAULT '{}',
    "FinalScore" numeric(6,3), "ConfidenceScore" numeric(6,3), "QualityScore" numeric(6,3),
    "DataScore" numeric(6,3), "ReliabilityScore" numeric(6,3), "ValidationScore" numeric(6,3),
    "CompletenessScore" numeric(6,3), "AiScore" numeric(6,3), "RiskScore" numeric(6,3),
    "PopularityScore" numeric(6,3), "PriorityScore" numeric(6,3), "ImportanceScore" numeric(6,3),
    "FreshnessScore" numeric(6,3), "AccuracyScore" numeric(6,3), "AnomalyScore" numeric(6,3),
    "EngagementScore" numeric(6,3), "HistoricalScore" numeric(6,3), "CoverageScore" numeric(6,3),
    "ConsistencyScore" numeric(6,3), "TrendScore" numeric(6,3), "ActivityScore" numeric(6,3),
    "SourceScore" numeric(6,3)
);

CREATE TABLE IF NOT EXISTS "DiscoveryQueue" (
    "Guid"              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"         timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"         timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"         boolean NOT NULL DEFAULT false,
    "Emoji"             text NOT NULL DEFAULT '',
    "Colors"            text NOT NULL DEFAULT '',
    "EntityType"        text,
    "SourceHint"        text,
    "DiscoveredFrom"    text,
    "Priority"          integer DEFAULT 50,
    "Status"            text DEFAULT 'queued',
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

CREATE TABLE IF NOT EXISTS "SourceCircuitBreaker" (
    "Guid"              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"         timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"         timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"         boolean NOT NULL DEFAULT false,
    "Emoji"             text NOT NULL DEFAULT '',
    "Colors"            text NOT NULL DEFAULT '',
    "SourceName"        text UNIQUE,
    "State"             text DEFAULT 'closed',
    "Failures"          integer DEFAULT 0,
    "LastFailureAt"     timestamptz,
    "ResetAt"           timestamptz,
    "FinalScore" numeric(6,3), "ConfidenceScore" numeric(6,3), "QualityScore" numeric(6,3),
    "DataScore" numeric(6,3), "ReliabilityScore" numeric(6,3), "ValidationScore" numeric(6,3),
    "CompletenessScore" numeric(6,3), "AiScore" numeric(6,3), "RiskScore" numeric(6,3),
    "PopularityScore" numeric(6,3), "PriorityScore" numeric(6,3), "ImportanceScore" numeric(6,3),
    "FreshnessScore" numeric(6,3), "AccuracyScore" numeric(6,3), "AnomalyScore" numeric(6,3),
    "EngagementScore" numeric(6,3), "HistoricalScore" numeric(6,3), "CoverageScore" numeric(6,3),
    "ConsistencyScore" numeric(6,3), "TrendScore" numeric(6,3), "ActivityScore" numeric(6,3),
    "SourceScore" numeric(6,3)
);

CREATE TABLE IF NOT EXISTS "ManualReviewQueue" (
    "Guid"              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"         timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"         timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"         boolean NOT NULL DEFAULT false,
    "Emoji"             text NOT NULL DEFAULT '',
    "Colors"            text NOT NULL DEFAULT '',
    "EntityTable"       text,
    "EntityGuid"        uuid,
    "Reason"            text,
    "Status"            text DEFAULT 'pending',
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

CREATE TABLE IF NOT EXISTS "AgentPromptCache" (
    "Guid"              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"         timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"         timestamptz NOT NULL DEFAULT now(),
    "IsDeleted"         boolean NOT NULL DEFAULT false,
    "Emoji"             text NOT NULL DEFAULT '',
    "Colors"            text NOT NULL DEFAULT '',
    "AgentType"         text,
    "PromptFile"        text,
    "PromptVersion"     text,
    "Content"           text,
    "FinalScore" numeric(6,3), "ConfidenceScore" numeric(6,3), "QualityScore" numeric(6,3),
    "DataScore" numeric(6,3), "ReliabilityScore" numeric(6,3), "ValidationScore" numeric(6,3),
    "CompletenessScore" numeric(6,3), "AiScore" numeric(6,3), "RiskScore" numeric(6,3),
    "PopularityScore" numeric(6,3), "PriorityScore" numeric(6,3), "ImportanceScore" numeric(6,3),
    "FreshnessScore" numeric(6,3), "AccuracyScore" numeric(6,3), "AnomalyScore" numeric(6,3),
    "EngagementScore" numeric(6,3), "HistoricalScore" numeric(6,3), "CoverageScore" numeric(6,3),
    "ConsistencyScore" numeric(6,3), "TrendScore" numeric(6,3), "ActivityScore" numeric(6,3),
    "SourceScore" numeric(6,3)
);
