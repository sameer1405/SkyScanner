using System;
using System.Collections.Generic;

namespace SkyScanner.SDK.Configuration.Constants
{
    public class SkyJson
    {
        public class FinalDataToGet
        {
            public double AmountMoney { get; set; }
            public DateTime DateDeparture { get; set; }
            public double DurationGoing { get; set; }
            public DateTime DateArrival { get; set; }
            public double DurationComingBack { get; set; }
            public double Rating { get; set; }
            public string UrlFinal { get; set; }
            public string UrlChecksum { get; set; }
            public List<Leg2> legParsed { get; set; }
        }




        public class Rootobject
        {
            public Query query { get; set; }
            public Context context { get; set; }
            public Itinerary[] itineraries { get; set; }
            public Leg2[] legs { get; set; }
            public Segment[] segments { get; set; }
            public Place[] places { get; set; }
            public Carrier[] carriers { get; set; }
            public Alliance[] alliances { get; set; }
            public object[] brands { get; set; }
            public Agent[] agents { get; set; }
            public Stats stats { get; set; }
            public object[] quote_requests { get; set; }
            public object[] quotes { get; set; }
            public Repro_Urls repro_urls { get; set; }
            public object[] rejected_itineraries { get; set; }
            public Plugin[] plugins { get; set; }
        }

        public class Query
        {
            public string market { get; set; }
            public string currency { get; set; }
            public string locale { get; set; }
            public string adults { get; set; }
            public object[] child_ages { get; set; }
            public string cabin_class { get; set; }
            public bool prefer_directs { get; set; }
            public Leg[] legs { get; set; }
            public string children { get; set; }
            public string infants { get; set; }
        }

        public class Leg
        {
            public string origin { get; set; }
            public object[] alternative_origins { get; set; }
            public string destination { get; set; }
            public object[] alternative_destinations { get; set; }
            public string date { get; set; }
        }

        public class Context
        {
            public string request_id { get; set; }
            public string session_id { get; set; }
        }

        public class Stats
        {
            public Itineraries itineraries { get; set; }
            public Leg1[] legs { get; set; }
            public Carriers carriers { get; set; }
            public Filter_Hints filter_hints { get; set; }
        }

        public class Itineraries
        {
            public string min_duration { get; set; }
            public string max_duration { get; set; }
            public string min_longest_itinerary_leg_duration { get; set; }
            public string max_longest_itinerary_leg_duration { get; set; }
            public Total total { get; set; }
            public Stops stops { get; set; }
            public bool has_change_airport_transfer { get; set; }
        }

        public class Total
        {
            public string count { get; set; }
            public float min_price { get; set; }
        }

        public class Stops
        {
            public Direct direct { get; set; }
            public One_Stop one_stop { get; set; }
            public Two_Plus_Stops two_plus_stops { get; set; }
        }

        public class Direct
        {
            public Total1 total { get; set; }
            public Ticket ticket { get; set; }
        }

        public class Total1
        {
            public string count { get; set; }
        }

        public class Ticket
        {
            public Single_Ticket single_ticket { get; set; }
            public Multi_Ticket_Non_Npt multi_ticket_non_npt { get; set; }
            public Multi_Ticket_Npt multi_ticket_npt { get; set; }
        }

        public class Single_Ticket
        {
            public string count { get; set; }
        }

        public class Multi_Ticket_Non_Npt
        {
            public string count { get; set; }
        }

        public class Multi_Ticket_Npt
        {
            public string count { get; set; }
        }

        public class One_Stop
        {
            public Total2 total { get; set; }
            public Ticket1 ticket { get; set; }
        }

        public class Total2
        {
            public string count { get; set; }
            public float min_price { get; set; }
        }

        public class Ticket1
        {
            public Single_Ticket1 single_ticket { get; set; }
            public Multi_Ticket_Non_Npt1 multi_ticket_non_npt { get; set; }
            public Multi_Ticket_Npt1 multi_ticket_npt { get; set; }
        }

        public class Single_Ticket1
        {
            public string count { get; set; }
            public float min_price { get; set; }
        }

        public class Multi_Ticket_Non_Npt1
        {
            public string count { get; set; }
        }

        public class Multi_Ticket_Npt1
        {
            public string count { get; set; }
        }

        public class Two_Plus_Stops
        {
            public Total3 total { get; set; }
            public Ticket2 ticket { get; set; }
        }

        public class Total3
        {
            public string count { get; set; }
            public float min_price { get; set; }
        }

        public class Ticket2
        {
            public Single_Ticket2 single_ticket { get; set; }
            public Multi_Ticket_Non_Npt2 multi_ticket_non_npt { get; set; }
            public Multi_Ticket_Npt2 multi_ticket_npt { get; set; }
        }

        public class Single_Ticket2
        {
            public string count { get; set; }
            public float min_price { get; set; }
        }

        public class Multi_Ticket_Non_Npt2
        {
            public string count { get; set; }
        }

        public class Multi_Ticket_Npt2
        {
            public string count { get; set; }
        }

        public class Carriers
        {
            public Single_Carriers[] single_carriers { get; set; }
            public Multiple_Carriers multiple_carriers { get; set; }
        }

        public class Multiple_Carriers
        {
            public string count { get; set; }
            public float min_price { get; set; }
        }

        public class Single_Carriers
        {
            public string id { get; set; }
            public string count { get; set; }
            public float min_price { get; set; }
        }

        public class Filter_Hints
        {
            public bool enable_direct_filter { get; set; }
            public bool enable_one_stop_filter { get; set; }
            public bool enable_two_plus_stops_filter { get; set; }
            public bool check_direct_filter { get; set; }
            public bool check_one_stop_filter { get; set; }
            public bool check_two_plus_stops_filter { get; set; }
            public bool show_npt_filter { get; set; }
            public bool check_npt_filter { get; set; }
        }

        public class Leg1
        {
            public string index { get; set; }
            public string min_duration { get; set; }
            public string max_duration { get; set; }
            public string count { get; set; }
            public Origin[] origins { get; set; }
            public Destination[] destinations { get; set; }
        }

        public class Origin
        {
            public string id { get; set; }
            public string count { get; set; }
        }

        public class Destination
        {
            public string id { get; set; }
            public string count { get; set; }
        }

        public class Repro_Urls
        {
            public string ps_repro_url { get; set; }
            public object[] lus_repro_urls { get; set; }
        }

        public class Itinerary
        {
            public string id { get; set; }
            public string[] leg_ids { get; set; }
            public Pricing_Options[] pricing_options { get; set; }
            public float score { get; set; }
        }

        public class Pricing_Options
        {
            public string[] agent_ids { get; set; }
            public Price price { get; set; }
            public string unpriced_type { get; set; }
            public Item[] items { get; set; }
        }

        public class Price
        {
            public double amount { get; set; }
            public string update_status { get; set; }
            public object last_updated { get; set; }
            public string quote_age { get; set; }
        }

        public class Item
        {
            public string agent_id { get; set; }
            public string url { get; set; }
            public string[] segment_ids { get; set; }
            public Price1 price { get; set; }
            public string transfer_protection { get; set; }
            public string max_redirect_age { get; set; }
            public Fare[] fares { get; set; }
            public string opaque_id { get; set; }
        }

        public class Price1
        {
            public double amount { get; set; }
            public string update_status { get; set; }
            public object last_updated { get; set; }
            public string quote_age { get; set; }
        }

        public class Fare
        {
            public string segment_id { get; set; }
            public string fare_basis_code { get; set; }
            public string booking_code { get; set; }
            public string fare_family { get; set; }
        }

        public class Leg2
        {
            public string id { get; set; }
            public string origin_place_id { get; set; }
            public string destination_place_id { get; set; }
            public DateTime departure { get; set; }
            public DateTime arrival { get; set; }
            public string[] segment_ids { get; set; }
            public double duration { get; set; }
            public int stop_count { get; set; }
            public string[] marketing_carrier_ids { get; set; }
            public string[] operating_carrier_ids { get; set; }
            public string[][] stop_ids { get; set; }
        }

        public class Segment
        {
            public string id { get; set; }
            public string origin_place_id { get; set; }
            public string destination_place_id { get; set; }
            public DateTime arrival { get; set; }
            public DateTime departure { get; set; }
            public string duration { get; set; }
            public string marketing_flight_number { get; set; }
            public string marketing_carrier_id { get; set; }
            public string operating_carrier_id { get; set; }
            public string mode { get; set; }
        }

        public class Place
        {
            public string id { get; set; }
            public string alt_id { get; set; }
            public string parent_id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string display_code { get; set; }
        }

        public class Carrier
        {
            public string id { get; set; }
            public string name { get; set; }
            public string alt_id { get; set; }
            public string display_code { get; set; }
            public string display_code_type { get; set; }
            public string alliance { get; set; }
        }

        public class Alliance
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Agent
        {
            public string id { get; set; }
            public string name { get; set; }
            public bool is_carrier { get; set; }
            public string update_status { get; set; }
            public bool optimised_for_mobile { get; set; }
            public bool live_update_allowed { get; set; }
            public string rating_status { get; set; }
            public float rating { get; set; }
            public string feedback_count { get; set; }
            public Rating_Breakdown rating_breakdown { get; set; }
        }

        public class Rating_Breakdown
        {
            public float reliable_prices { get; set; }
            public float clear_extra_fees { get; set; }
            public string customer_service { get; set; }
            public float ease_of_booking { get; set; }
            public float other { get; set; }
        }

        public class Plugin
        {
            public string type { get; set; }
            public Itineraries1 itineraries { get; set; }
        }

        public class Itineraries1
        {
        }
    }
}
