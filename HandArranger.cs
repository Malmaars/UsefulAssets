class HandArranger
{
    //pseudocode, partly

    void ArrangeHand()
    {
        //move each card in hand to its correct position;
        //I need to know the extents of the physical width of the hand
        float totalWidth = Vector3.Distance(leftSidePosition, rightSidePosition);

        float distanceBetweenCards = totalWidth / (cardsInHand.Count + 1);
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            Vector3 handPos = leftSide.transform.position + (Vector3.right * distanceBetweenCards * (i + 1) + Vector3.back * (i * 0.15f));
            handPos = new Vector3(handPos.x, handPos.y, -i * 0.2f);
            cardsInHand[i].transform.position = Vector3.Lerp(cardsInHand[i].transform.position, handPos, Time.deltaTime * 4);
        }
    }
}