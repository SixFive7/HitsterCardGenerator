export async function fetchHealth(): Promise<{ status: string; timestamp: string }> {
  const response = await fetch('/api/health')
  return response.json()
}
