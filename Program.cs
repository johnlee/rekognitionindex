using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;

namespace RekogIndexer
{
    class Program
    {
        static AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(Amazon.RegionEndpoint.USWest2);
        static String collectionId = "llnlfacerekogtest1collection";
        static String bucket = "llnlfacerekogtest1bucket";

        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("Facial Recognition using AWS Rekognition");
            Console.WriteLine("");
            Console.WriteLine("Select an option below:");
            Console.WriteLine(" 1 - Index a photo in S3");
            Console.WriteLine(" 2 - List faces in collection");
            Console.WriteLine("");
            Console.Write("Option: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.Write("Enter S3 photo filename: ");
                    var s3filename = Console.ReadLine();
                    IndexPhoto(s3filename);
                    break;
                case "2":
                    ListFaces();
                    break;
                default:
                    Console.WriteLine("Invalid option. Exiting...");
                    break;
            }
            Console.WriteLine();
            return;
        }

        private static void IndexPhoto(string photo)
        {
            Console.WriteLine("Attempting to index S3 photo file " + photo);

            Image image = new Image()
            {
                S3Object = new S3Object()
                {
                    Bucket = bucket,
                    Name = photo
                }
            };

            IndexFacesRequest request = new IndexFacesRequest()
            {
                Image = image,
                CollectionId = collectionId,
                ExternalImageId = photo,
                DetectionAttributes = new List<String>() { "ALL" }
            };

            IndexFacesResponse response = rekognitionClient.IndexFacesAsync(request).Result;

            Console.WriteLine(photo + " added");

            foreach (FaceRecord faceRecord in response.FaceRecords)
                Console.WriteLine("Face detected: Faceid is " +
                   faceRecord.Face.FaceId);
        }

        private static void ListFaces()
        {
            ListFacesResponse response = null;
            Console.WriteLine("Faces in collection " + collectionId);

            String paginationToken = null;
            do
            {
                if (response != null)
                    paginationToken = response.NextToken;

                ListFacesRequest request = new ListFacesRequest()
                {
                    CollectionId = collectionId,
                    MaxResults = 1,
                    NextToken = paginationToken
                };

                response = rekognitionClient.ListFacesAsync(request).Result;
                foreach (Face face in response.Faces)
                    Console.WriteLine(face.FaceId);
            } while (response != null && !String.IsNullOrEmpty(response.NextToken));
        }
    }
}
