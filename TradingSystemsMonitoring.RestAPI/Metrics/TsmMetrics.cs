using Prometheus;

namespace TradingSystemsMonitoring.RestAPI.Metrics
{
    /// <summary>
    /// Central definitions for TSM Prometheus metrics.
    /// </summary>
    public static class TsmMetrics
    {
        public static readonly Counter HttpRequestsFailedTotal = global::Prometheus.Metrics.CreateCounter(
            "tsm_http_requests_failed_total",
            "Total number of HTTP requests that resulted in 4xx or 5xx response.",
            new CounterConfiguration { LabelNames = new[] { "method", "route", "status_code" } });

        public static readonly Counter KafkaMessagesProcessedTotal = global::Prometheus.Metrics.CreateCounter(
            "tsm_kafka_messages_processed_total",
            "Total number of Kafka messages processed.",
            new CounterConfiguration { LabelNames = new[] { "topic", "status" } });

        public static readonly Counter KafkaDlqSentTotal = global::Prometheus.Metrics.CreateCounter(
            "tsm_kafka_dlq_sent_total",
            "Total number of messages sent to the dead-letter queue.",
            new CounterConfiguration { LabelNames = new[] { "topic", "dlq_topic" } });

        public static readonly Histogram KafkaProcessingDurationSeconds = global::Prometheus.Metrics.CreateHistogram(
            "tsm_kafka_processing_duration_seconds",
            "Duration of Kafka message processing in seconds.",
            new HistogramConfiguration
            {
                LabelNames = new[] { "topic" },
                Buckets = Histogram.ExponentialBuckets(0.001, 2, 12)
            });

        public static readonly Counter KafkaConsumerErrorsTotal = global::Prometheus.Metrics.CreateCounter(
            "tsm_kafka_consumer_errors_total",
            "Total number of Kafka consumer errors.",
            new CounterConfiguration { LabelNames = new[] { "topic", "phase" } });

        public static readonly Counter ApiTradesRequestsTotal = global::Prometheus.Metrics.CreateCounter(
            "tsm_api_trades_requests_total",
            "Total number of trades API requests.",
            new CounterConfiguration { LabelNames = new[] { "endpoint" } });

        public static readonly Counter ApiLogRecordsRequestsTotal = global::Prometheus.Metrics.CreateCounter(
            "tsm_api_log_records_requests_total",
            "Total number of log records API requests.");

        public static readonly Counter SignalrMessagesSentTotal = global::Prometheus.Metrics.CreateCounter(
            "tsm_signalr_messages_sent_total",
            "Total number of messages broadcast to SignalR clients (live trading data).");

        public static readonly Counter SignalrMessagesSendErrorsTotal = global::Prometheus.Metrics.CreateCounter(
            "tsm_signalr_messages_send_errors_total",
            "Total number of failed SignalR message broadcasts.");
    }
}
