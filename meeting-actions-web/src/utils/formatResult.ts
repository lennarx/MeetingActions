import type { ParsedResult } from '../types/api.ts';

/**
 * Formats the parsed result object into plain text with sections
 * Used for copying to clipboard
 */
export function formatResult(parsed: ParsedResult): string {
  const sections: string[] = [];

  if (parsed.decisions && parsed.decisions.length > 0) {
    sections.push('## Decisions\n' + parsed.decisions.map(d => `- ${d}`).join('\n'));
  }

  if (parsed.actions && parsed.actions.length > 0) {
    sections.push('## Actions\n' + parsed.actions.map(a => `- ${a}`).join('\n'));
  }

  if (parsed.implicitDates && parsed.implicitDates.length > 0) {
    sections.push('## Implicit Dates\n' + parsed.implicitDates.map(d => `- ${d}`).join('\n'));
  }

  if (parsed.risks && parsed.risks.length > 0) {
    sections.push('## Risks\n' + parsed.risks.map(r => `- ${r}`).join('\n'));
  }

  if (parsed.openQuestions && parsed.openQuestions.length > 0) {
    sections.push('## Open Questions\n' + parsed.openQuestions.map(q => `- ${q}`).join('\n'));
  }

  return sections.join('\n\n');
}
