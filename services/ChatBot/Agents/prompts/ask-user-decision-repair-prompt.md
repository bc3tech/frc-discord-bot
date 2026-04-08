The previous ask_user workflow decision did not pass semantic review and must be repaired before anything is shown to the user.

Turn context:
[[TURN_CONTEXT]]

Current user message:
[[CURRENT_USER_MESSAGE]]

Previous workflow decision:
- next_step: [[NEXT_STEP]]
- question: [[QUESTION]]
- reason: [[REASON]]
- messageToUser: [[MESSAGE_TO_USER]]

Required fixes:
[[FEEDBACK]]

Re-evaluate the request using the existing grounded conversation context and available tools.
If the user is clearly referring to recently named people or to parents/guardians already established in context, resolve that referent from context instead of asking them to repeat the names.
If clarification is still genuinely required, emit ask_user with a tighter question.
If clarification is not yet necessary, continue the workflow and emit query_local or final as appropriate.
Do not mention this review step to the user.
