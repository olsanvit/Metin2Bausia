-- ============================================================
-- 08: správci aplikace (sub-admini)
--
-- Proč: dřív byl seznam povolených Google účtů jen v appsettings na QNAPu — přidat správce
-- znamenalo měnit produkční konfiguraci. Teď ho spravuje admin na /admin/access.
-- Super-admin (Admin:Email, olsanskyvitek) se doplní při startu aplikace — v kódu ani tady
-- natvrdo není, a nejde ho odebrat.
-- Odebrání = RemovedAt, ne DELETE: historie zůstává pro audit (zápis i do ApprovalLog).
-- ============================================================

CREATE TABLE IF NOT EXISTS "AdminUsers" (
    "Guid"         uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt"    timestamptz NOT NULL DEFAULT now(),
    "UpdatedAt"    timestamptz NOT NULL DEFAULT now(),
    "Email"        citext NOT NULL,
    "IsSuperAdmin" boolean NOT NULL DEFAULT false,
    "CreatedBy"    text,
    "RemovedAt"    timestamptz,
    "RemovedBy"    text
);

-- Jeden aktivní záznam na e-mail; po odebrání lze stejný e-mail přidat znovu
CREATE UNIQUE INDEX IF NOT EXISTS "UX_AdminUsers_Email_Active" ON "AdminUsers" ("Email") WHERE "RemovedAt" IS NULL;
