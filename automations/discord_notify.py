#!/usr/bin/env python3
"""
discord_notify.py
=================
Sends rich Discord webhook notifications for CyberWorld / FLLC GitHub events.

Triggered by: .github/workflows/discord-notify.yml
Supports events: push, pull_request, workflow_run, release, manual
"""

from __future__ import annotations

import json
import os
import sys
from datetime import datetime, timezone

import requests


# ---------------------------------------------------------------------------
# Configuration (injected via GitHub Actions secrets / env vars)
# ---------------------------------------------------------------------------
DISCORD_WEBHOOK_URL = os.environ.get("DISCORD_WEBHOOK_URL", "")
GITHUB_EVENT_NAME   = os.environ.get("GITHUB_EVENT_NAME", "manual")
GITHUB_REPOSITORY   = os.environ.get("GITHUB_REPOSITORY", "Personfu/CYBERWORLDSOURCECODE")
GITHUB_ACTOR        = os.environ.get("GITHUB_ACTOR", "unknown")
GITHUB_REF          = os.environ.get("GITHUB_REF", "refs/heads/main")
GITHUB_SHA          = os.environ.get("GITHUB_SHA", "")
GITHUB_RUN_ID       = os.environ.get("GITHUB_RUN_ID", "")
GITHUB_SERVER_URL   = os.environ.get("GITHUB_SERVER_URL", "https://github.com")
CUSTOM_MESSAGE      = os.environ.get("CUSTOM_MESSAGE", "")

# Colour palette (Discord decimal colours)
COLOURS = {
    "push":         0x00FFCC,   # neon cyan
    "pull_request": 0x7A00FF,   # electric violet
    "workflow_run": 0x00FF88,   # matrix green
    "release":      0xFFD700,   # gold
    "manual":       0x00BFFF,   # deep sky blue
    "error":        0xFF4466,   # red
}


# ---------------------------------------------------------------------------
# Embed builders
# ---------------------------------------------------------------------------
def _repo_url() -> str:
    return f"{GITHUB_SERVER_URL}/{GITHUB_REPOSITORY}"


def _run_url() -> str:
    if GITHUB_RUN_ID:
        return f"{_repo_url()}/actions/runs/{GITHUB_RUN_ID}"
    return _repo_url()


def _branch_name() -> str:
    return GITHUB_REF.replace("refs/heads/", "").replace("refs/tags/", "tag: ")


def _short_sha() -> str:
    return GITHUB_SHA[:7] if GITHUB_SHA else "unknown"


def build_push_embed() -> dict:
    return {
        "title": f"🚀 Push to `{_branch_name()}`",
        "description": (
            f"**Actor:** `{GITHUB_ACTOR}`\n"
            f"**SHA:** [`{_short_sha()}`]({_repo_url()}/commit/{GITHUB_SHA})\n"
            f"**Repo:** [{GITHUB_REPOSITORY}]({_repo_url()})"
        ),
        "color": COLOURS["push"],
        "footer": {"text": f"CyberWorld CI • {datetime.now(tz=timezone.utc).strftime('%Y-%m-%d %H:%M UTC')}"},
        "url": _repo_url(),
    }


def build_pr_embed() -> dict:
    event_path = os.environ.get("GITHUB_EVENT_PATH", "")
    pr_title  = "Pull Request"
    pr_url    = _repo_url()
    pr_number = ""
    pr_action = os.environ.get("PR_ACTION", "opened")

    if event_path and os.path.exists(event_path):
        try:
            with open(event_path) as fh:
                ev = json.load(fh)
            pr = ev.get("pull_request", {})
            pr_title  = pr.get("title", pr_title)
            pr_url    = pr.get("html_url", pr_url)
            pr_number = f"#{pr.get('number', '')}"
            pr_action = ev.get("action", pr_action)
        except (json.JSONDecodeError, OSError):
            pass

    action_emoji = {"opened": "🟢", "closed": "🔴", "merged": "💜", "reopened": "🟡"}.get(pr_action, "🔵")

    return {
        "title": f"{action_emoji} PR {pr_number} {pr_action.capitalize()}: {pr_title}",
        "description": (
            f"**Actor:** `{GITHUB_ACTOR}`\n"
            f"**Branch:** `{_branch_name()}`\n"
            f"**Repo:** [{GITHUB_REPOSITORY}]({_repo_url()})"
        ),
        "color": COLOURS["pull_request"],
        "url": pr_url,
        "footer": {"text": f"CyberWorld CI • {datetime.now(tz=timezone.utc).strftime('%Y-%m-%d %H:%M UTC')}"},
    }


def build_workflow_embed(conclusion: str = "") -> dict:
    status_emoji = {"success": "✅", "failure": "❌", "cancelled": "⚠️"}.get(conclusion, "🔄")
    colour       = COLOURS["error"] if conclusion == "failure" else COLOURS["workflow_run"]

    return {
        "title": f"{status_emoji} Workflow {conclusion or 'started'}: `{GITHUB_EVENT_NAME}`",
        "description": (
            f"**Actor:** `{GITHUB_ACTOR}`\n"
            f"**Run:** [View Run]({_run_url()})\n"
            f"**Repo:** [{GITHUB_REPOSITORY}]({_repo_url()})"
        ),
        "color": colour,
        "url": _run_url(),
        "footer": {"text": f"CyberWorld CI • {datetime.now(tz=timezone.utc).strftime('%Y-%m-%d %H:%M UTC')}"},
    }


def build_release_embed() -> dict:
    tag = GITHUB_REF.replace("refs/tags/", "")
    return {
        "title": f"🎉 New Release: `{tag}`",
        "description": (
            f"**Actor:** `{GITHUB_ACTOR}`\n"
            f"**Repo:** [{GITHUB_REPOSITORY}]({_repo_url()})\n"
            f"[View Release]({_repo_url()}/releases/tag/{tag})"
        ),
        "color": COLOURS["release"],
        "url": f"{_repo_url()}/releases/tag/{tag}",
        "footer": {"text": f"CyberWorld CI • {datetime.now(tz=timezone.utc).strftime('%Y-%m-%d %H:%M UTC')}"},
    }


def build_manual_embed(message: str) -> dict:
    return {
        "title": "📣 CyberWorld Manual Notification",
        "description": message or "Manual notification triggered.",
        "color": COLOURS["manual"],
        "url": _repo_url(),
        "footer": {"text": f"CyberWorld • {datetime.now(tz=timezone.utc).strftime('%Y-%m-%d %H:%M UTC')}"},
    }


# ---------------------------------------------------------------------------
# Dispatch
# ---------------------------------------------------------------------------
def build_embed() -> dict:
    conclusion = os.environ.get("WORKFLOW_CONCLUSION", "")

    dispatch: dict[str, object] = {
        "push":         build_push_embed,
        "pull_request": build_pr_embed,
        "workflow_run": lambda: build_workflow_embed(conclusion),
        "release":      build_release_embed,
    }

    builder = dispatch.get(GITHUB_EVENT_NAME)
    if builder:
        return builder()  # type: ignore[operator]
    return build_manual_embed(CUSTOM_MESSAGE)


# ---------------------------------------------------------------------------
# Send
# ---------------------------------------------------------------------------
def send_notification(embed: dict) -> None:
    if not DISCORD_WEBHOOK_URL:
        print("⚠️  DISCORD_WEBHOOK_URL is not set — skipping notification.")
        return

    payload = {
        "username": "CyberWorld CI",
        "avatar_url": "https://github.com/Personfu/CYBERWORLDSOURCECODE/raw/main/DevAssets/PromoArt/PenguinRender.png",
        "embeds": [embed],
    }

    resp = requests.post(DISCORD_WEBHOOK_URL, json=payload, timeout=15)
    if resp.status_code in (200, 204):
        print(f"✅ Discord notification sent (HTTP {resp.status_code})")
    else:
        print(f"❌ Discord notification failed: HTTP {resp.status_code} — {resp.text}", file=sys.stderr)
        sys.exit(1)


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------
def main() -> None:
    print(f"📨 Discord Notifier — event: {GITHUB_EVENT_NAME}")
    embed = build_embed()
    send_notification(embed)


if __name__ == "__main__":
    main()
